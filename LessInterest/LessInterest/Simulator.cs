namespace LessInterest;

public class Simulator(Config config, Boolean printSimulation)
{
	private Config config = config;
	private Boolean printSimulation = printSimulation;

	public Report Process(
		IList<Decimal> balancesPt, Decimal nubankLimit, Decimal c6Limit,
		IList<Int32> installmentsCounts, IList<Int32> installmentsDelays
	)
	{
		var totalInterest = 0m;
		var reInstallments = new List<Decimal>();

		return process(
			0,
			balancesPt, nubankLimit, c6Limit,
			installmentsCounts, installmentsDelays,
			installmentsCounts[0], installmentsDelays[0],
			totalInterest, reInstallments, new Simulation()
		)!;
	}

	public Simulation? ProcessAll(
		IList<Decimal> balancesPt, Decimal nubankLimit, Decimal c6Limit
	)
	{
		var totalInterest = 0m;
		var reInstallments = new List<Decimal>();

		return oneOrAll(
			(count, delay) => process(
				0,
				balancesPt, nubankLimit, c6Limit,
				null, null,
				count, delay,
				totalInterest, reInstallments, new Simulation()
			)
		);
	}

	private Simulation? process(
		Int32 monthIndex,
		IList<Decimal> balancesPt, Decimal nubankLimit, Decimal c6Limit,
		IList<Int32>? installmentsCounts, IList<Int32>? installmentsDelays,
		Int32 installmentCount, Int32 installmentDelay,
		Decimal totalInterest, IList<Decimal> reInstallments, Simulation simulation
	)
	{
		simulation = simulation.NewMonth();

		simulation.MonthLabel = config.Months[monthIndex];
		var nextMonthIndex = monthIndex + 1;

		var printStep = monthIndex < 7;

		if (printStep)
		{
			Console.WriteLine($"{new String('-', monthIndex)} {simulation.MonthLabel} {installmentCount}x after {installmentDelay} months:");
		}

		if (reInstallments.Count <= monthIndex)
			reInstallments.Add(0);

		if (config.NubankInstallments.Count <= monthIndex)
			config.NubankInstallments.Add(0);

		if (config.C6Installments.Count <= monthIndex)
			config.C6Installments.Add(0);

		simulation.NubankInstallments = config.NubankInstallments[monthIndex];
		simulation.NubankLimit = nubankLimit + config.NubankInstallments[monthIndex];

		simulation.C6Installments = config.C6Installments[monthIndex];
		simulation.C6Limit = c6Limit + config.C6Installments[monthIndex];

		simulation.Limit = simulation.NubankLimit + simulation.C6Limit;

		simulation.Salary = config.Salary[monthIndex];
		simulation.SpentPT = config.SpentPT[monthIndex];

		simulation.BalancePTInitial = balancesPt[monthIndex];

		simulation.BalancePTFinal =
			simulation.BalancePTInitial
			+ config.Salary[monthIndex]
			- config.SpentPT[monthIndex];

		simulation.BalancePTBR =
			simulation.BalancePTFinal * config.Currency;

		simulation.SalaryBR = 0;
		simulation.SpentBR = config.SpentBR[monthIndex];
		simulation.NubankInstallments = config.NubankInstallments[monthIndex];
		simulation.C6Installments = config.C6Installments[monthIndex];
		simulation.ReInstallments = reInstallments[monthIndex];

		simulation.BalanceBR =
			config.SpentBR[monthIndex]
			+ config.NubankInstallments[monthIndex]
			+ config.C6Installments[monthIndex]
			+ reInstallments[monthIndex];

		var interest = 0m;

		if (simulation.BalanceBR > simulation.BalancePTBR)
		{
			simulation.ReInstallmentNeeded = simulation.BalanceBR - simulation.BalancePTBR;
			interest = config.Interests[installmentCount - 1][installmentDelay];
		}
		else
		{
			simulation.ReInstallmentNeeded = 0;
			
		}

		simulation.ReInstallmentTotal = Math.Ceiling(
			simulation.ReInstallmentNeeded * interest / installmentCount * 100
		) * installmentCount / 100;

		if (simulation.ReInstallmentTotal > simulation.Limit)
		{
			if (installmentsCounts == null && installmentsDelays == null)
			{
				if (printStep)
					Console.WriteLine(" WRONG");
				return null;
			}

			simulation.ReInstallmentTotal = simulation.Limit;
			simulation.ReInstallmentAllowed = simulation.ReInstallmentTotal / interest;
		}
		else
		{
			simulation.ReInstallmentAllowed = simulation.ReInstallmentNeeded;
		}

		totalInterest += (simulation.ReInstallmentTotal - simulation.ReInstallmentAllowed);

		simulation.ReInstallmentPart = simulation.ReInstallmentTotal / installmentCount;

		var balanceBRNext = simulation.BalancePTBR - simulation.BalanceBR + simulation.ReInstallmentAllowed;
		simulation.BalancePTNext = Math.Round(balanceBRNext / config.Currency, 2);
		balancesPt.Add(simulation.BalancePTNext);

		simulation.NubankNewLimit = simulation.NubankLimit - simulation.ReInstallmentTotal;

		var nextReInstallment = nextMonthIndex + installmentDelay;
		while (reInstallments.Count <= nextReInstallment + installmentCount)
		{
			reInstallments.Add(0);
		}

		for (var i = 0; i < installmentCount; i++)
		{
			reInstallments[i + nextReInstallment] += simulation.ReInstallmentPart;
		}

		simulation.Total = totalInterest;

		if (nextMonthIndex == config.Months.Count)
		{
			if (printSimulation)
			{
				simulation.Print(Console.Write);
			}

			return simulation;
		}

		var childSimulation = oneOrAll(
			(count, delay) => process(
				nextMonthIndex,
				balancesPt, simulation.NubankNewLimit, simulation.C6Limit,
				installmentsCounts, installmentsDelays,
				count, delay,
				totalInterest, reInstallments, simulation
			),
			nextMonthIndex, installmentsCounts, installmentsDelays
		);

		if (printStep)
		{
			if (childSimulation == null)
				Console.WriteLine("WRONG =(");
			else
				Console.WriteLine($"RIGHT =) {totalInterest}");
		}

		return childSimulation;
	}

	private Simulation? oneOrAll(
		Func<Int32, Int32, Simulation?> execute,
		Int32? index = null, IList<Int32>? counts = null, IList<Int32>? delays = null
	)
	{
		if (
			index.HasValue
			&& counts != null
			&& delays != null
			&& counts.Count > index
			&& delays.Count > index
		)
		{
			return execute(counts[index.Value], delays[index.Value]);
		}

		Simulation? chosen = null;

		for (var delay = 0; delay <= 2; delay++)
		{
			for (var count = 1; count <= 12; count++)
			{
				var simulation = execute(count, delay);

				if (simulation == null) continue;

				if (chosen == null || chosen.Total > simulation.Total)
				{
					chosen = simulation;
				}
			}
		}

		return chosen;
	}
}
