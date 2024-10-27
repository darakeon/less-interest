namespace LessInterest;

public class Simulator(
	Config config,
	Action<String>? write = null
)
{
	private Config config = config;
	private Action<String> write = (text) => write?.Invoke(text);

	private static readonly String multiWrongPath = Path.Combine("..", "..", "..", "logs", "wrong.log");
	private static IList<String> multiWrong = getWrongs();

	public async Task<Simulation> Process(
		IList<Decimal> balancesPt, Decimal nubankLimit, Decimal c6Limit,
		IList<Int32> installmentsCounts, IList<Int32> installmentsDelays
	)
	{
		return (await process(balancesPt, nubankLimit, c6Limit))!;
	}

	public async Task<Simulation?> ProcessAll(
		IList<Decimal> balancesPt, Decimal nubankLimit, Decimal c6Limit, String multiKey
	)
	{
		return await oneOrAll(
			async (count, delay) => await process(
				balancesPt, nubankLimit, c6Limit, multiKey, count, delay
			),
			multiKey
		);
	}

	private async Task<Simulation?> process(
		IList<Decimal> balancesPt, Decimal nubankLimit, Decimal c6Limit, String? multiKey = null,
		Int32? chosenInstallmentCount = null, Int32? chosenInstallmentDelay = null,
		Decimal totalInterest = 0, IList<Decimal>? reInstallments = null,
		Simulation? simulation = null, Boolean isTarget = true
	)
	{
		simulation = simulation == null
			? new Simulation()
			: simulation.NewMonth();

		var monthIndex = simulation.MonthIndex;
		var nextMonthIndex = monthIndex + 1;

		var installmentCount =
			chosenInstallmentCount
			?? config.InitialInstallmentsCounts[monthIndex];

		var installmentDelay =
			chosenInstallmentDelay
			?? config.InitialInstallmentsDelays[monthIndex];

		if (multiKey != null)
		{
			multiKey += $"_{monthIndex}x{installmentCount}+{installmentDelay}";

			if (multiWrong.Contains(multiKey))
			{
				return null;
			}
		}

		reInstallments = reInstallments == null
			? new List<Decimal>()
			: reInstallments.ToList();

		simulation.MonthLabel = config.Months[monthIndex];

		isTarget = isTarget
		    && config.InitialInstallmentsCounts[monthIndex] == installmentCount
		    && config.InitialInstallmentsDelays[monthIndex] == installmentDelay;

		if (isTarget || monthIndex < 3)
		{
			write($"{new String('-', monthIndex + 1)} {simulation.MonthLabel} {installmentCount}x after {installmentDelay} months:");
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
			if (multiKey != null)
			{
				if (isTarget)
					write("WRONG");
				
				await setWrong(multiKey);

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
			simulation.Print(write);
			return simulation;
		}

		return await oneOrAll(
			(count, delay) => process(
				balancesPt, simulation.NubankNewLimit, simulation.C6Limit, multiKey,
				count, delay,
				totalInterest, reInstallments,
				simulation, isTarget
			), multiKey
		);
	}

	private async Task<Simulation?> oneOrAll(
		Func<Int32?, Int32?, Task<Simulation?>> execute, String? multiKey
	)
	{
		if (multiKey == null)
		{
			return await execute(null, null);
		}

		var simulations = new Task<Simulation?>[3*12];

		for (var delay = 0; delay <= 2; delay++)
		{
			for (var count = 1; count <= 12; count++)
			{
				var simulation = execute(count, delay);

				var index = delay * 12 + count - 1;
				simulations[index] = simulation;
			}
		}

		Task.WaitAll(simulations);

		var lowestSimulation = simulations.Select(
			t => t.Result
		).Where(
			s => s != null
		).OrderBy(
			s => s.Total
		).FirstOrDefault();

		if (lowestSimulation == null)
			await setWrong(multiKey);

		return lowestSimulation;
	}

	private static async Task setWrong(String multiKey)
	{
		await File.AppendAllLinesAsync(multiWrongPath, new[] { multiKey });
	}

	private static IList<String> getWrongs()
	{
		return File.Exists(multiWrongPath) 
			? File.ReadAllLines(multiWrongPath) 
			: Array.Empty<String>();
	}
}
