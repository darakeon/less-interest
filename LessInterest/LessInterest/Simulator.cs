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
			totalInterest, reInstallments, new Report()
		)!;
	}

	public Report? ProcessAll(
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
				totalInterest, reInstallments, new Report()
			)
		);
	}

	private Report? process(
		Int32 monthIndex,
		IList<Decimal> balancesPt, Decimal nubankLimit, Decimal c6Limit,
		IList<Int32>? installmentsCounts, IList<Int32>? installmentsDelays,
		Int32 installmentCount, Int32 installmentDelay,
		Decimal totalInterest, IList<Decimal> reInstallments, Report simulation
	)
	{
		var monthLabel = config.Months[monthIndex];
		var nextMonthIndex = monthIndex + 1;

		var dashes = new String('-', monthIndex);
		Console.Write($"{dashes} {monthLabel} {installmentCount}x after {installmentDelay} months:");

		if (reInstallments.Count <= monthIndex)
			reInstallments.Add(0);

		if (config.C6Installments.Count <= monthIndex)
			config.C6Installments.Add(0);

		if (config.NubankInstallments.Count <= monthIndex)
			config.NubankInstallments.Add(0);

		simulation.Add(monthIndex, new Field("month", monthLabel));

		simulation.Add(monthIndex, new Field("nubank_installments", config.NubankInstallments[monthIndex]));
		nubankLimit += config.NubankInstallments[monthIndex];
		simulation.Add(monthIndex, new Field("nubank_limit", nubankLimit));

		simulation.Add(monthIndex, new Field("c6_installments", config.C6Installments[monthIndex]));
		c6Limit += config.C6Installments[monthIndex];
		simulation.Add(monthIndex, new Field("c6_limit", c6Limit));

		var limit = nubankLimit + c6Limit;
		simulation.Add(monthIndex, new Field("limit", limit));

		simulation.Add(monthIndex, new Field("salary", config.Salary[monthIndex]));
		simulation.Add(monthIndex, new Field("spent_pt", config.SpentPT[monthIndex]));

		var balancePt = balancesPt[monthIndex];

		simulation.Add(monthIndex, new Field("balance_pt", balancePt));

		balancePt = balancePt + config.Salary[monthIndex] - config.SpentPT[monthIndex];
		simulation.Add(monthIndex, new Field("balance_pt_left", balancePt));

		var balancePtBr = balancePt * config.Currency;
		simulation.Add(monthIndex, new Field("balance_pt_br", balancePtBr));

		simulation.Add(monthIndex, new Field("salary_br", 0));
		simulation.Add(monthIndex, new Field("spent_br", config.SpentBR[monthIndex]));
		simulation.Add(monthIndex, new Field("nubank_installments", config.NubankInstallments[monthIndex]));
		simulation.Add(monthIndex, new Field("c6_installments", config.C6Installments[monthIndex]));
		simulation.Add(monthIndex, new Field("reInstallments", reInstallments[monthIndex]));

		var balanceBr =
			config.SpentBR[monthIndex]
			+ config.NubankInstallments[monthIndex]
			+ config.C6Installments[monthIndex]
			+ reInstallments[monthIndex];

		simulation.Add(monthIndex, new Field("balance_br", balanceBr));

		var reInstallment = 0m;
		var interest = 0m;

		if (balanceBr > balancePtBr)
		{
			reInstallment = balanceBr - balancePtBr;
			interest = config.Interests[installmentCount - 1][installmentDelay];
		}

		simulation.Add(monthIndex, new Field("re_installment", reInstallment));

		var reInstallmentTotal = Math.Ceiling(
			reInstallment * interest / installmentCount * 100
		) * installmentCount / 100;

		if (reInstallmentTotal > limit)
		{
			if (installmentsCounts == null && installmentsDelays == null)
			{
				Console.WriteLine(" WRONG");
				return null;
			}

			reInstallmentTotal = limit;
			reInstallment = reInstallmentTotal / interest;
		}
		Console.WriteLine();

		totalInterest += (reInstallmentTotal - reInstallment);

		var reInstallmentPart = reInstallmentTotal / installmentCount;

		simulation.Add(monthIndex, new Field("re_installment_total", reInstallmentTotal));
		simulation.Add(monthIndex, new Field("re_installment_part", reInstallmentPart));

		balancePt = Math.Round((balancePtBr - balanceBr + reInstallment) / config.Currency, 2);
		balancesPt.Add(balancePt);

		nubankLimit -= reInstallmentTotal;

		var nextReInstallment = nextMonthIndex + installmentDelay;
		while (reInstallments.Count <= nextReInstallment + installmentCount)
		{
			reInstallments.Add(0);
		}

		for (var i = 0; i < installmentCount; i++)
		{
			reInstallments[i + nextReInstallment] += reInstallmentPart;
		}

		simulation.Total = new Field("total_interest", totalInterest);

		if (printSimulation)
		{
			simulation.Print(Console.Write);
		}

		if (nextMonthIndex == config.Months.Count)
			return simulation;
		
		return oneOrAll(
			(count, delay) => process(
				nextMonthIndex,
				balancesPt, nubankLimit, c6Limit,
				installmentsCounts, installmentsDelays,
				count, delay,
				totalInterest, reInstallments, simulation
			),
			nextMonthIndex, installmentsCounts, installmentsDelays
		);
	}

	private Report? oneOrAll(
		Func<Int32, Int32, Report?> execute,
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

		Report? chosen = null;

		for (var delay = 0; delay <= 2; delay++)
		{
			for (var count = 1; count <= 12; count++)
			{
				var simulation = execute(count, delay);;

				if (simulation == null) continue;

				if (chosen == null || chosen.Total.Decimal > simulation.Total.Decimal)
				{
					chosen = simulation;
				}
			}
		}

		return chosen;
	}
}
