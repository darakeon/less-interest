using System.Collections.Immutable;

namespace LessInterest;

public class Simulator(
	Config config,
	Action<String>? write = null
)
{
	private Config config = config;
	private Action<String> write = (text) => write?.Invoke(text);

	private static readonly String multiWrongPath = Path.Combine("..", "..", "..", "logs", "wrong.log");
	private static ISet<String> multiWrong = getWrongs();

	public async Task<ISimulation> Process(
		IList<Single> balancesPt, Single nubankLimit, Single c6Limit,
		IList<Int16> installmentsCounts, IList<Int16> installmentsDelays
	)
	{
		return (await process(balancesPt, nubankLimit, c6Limit));
	}

	public async Task<ISimulation> ProcessAll(
		IList<Single> balancesPt, Single nubankLimit, Single c6Limit, String multiKey
	)
	{
		return await oneOrAll(
			async (count, delay) => await process(
				balancesPt, nubankLimit, c6Limit, multiKey, count, delay
			),
			0,
			multiKey
		);
	}

	private async Task<ISimulation> process(
		IList<Single> balancesPt, Single nubankLimit, Single c6Limit, String? multiKey = null,
		Int16? chosenInstallmentCount = null, Int16? chosenInstallmentDelay = null,
		Single totalInterest = 0, IList<Single>? reInstallments = null,
		ISimulation? simulation = null, Boolean isTarget = true
	)
	{
		simulation = simulation == null
			? new SimpleSimulation()
			: simulation.NewMonth();

		var monthIndex = simulation.MonthIndex;
		var nextMonthIndex = (Int16)(monthIndex + 1);

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
				simulation.Valid = false;
				return simulation;
			}
		}

		reInstallments = reInstallments == null
			? new List<Single>()
			: reInstallments.ToList();

		simulation.MonthLabel = config.Months[monthIndex];

		isTarget = isTarget
		    && config.InitialInstallmentsCounts[monthIndex] == installmentCount
		    && config.InitialInstallmentsDelays[monthIndex] == installmentDelay;

		if (isTarget || monthIndex < 5)
		{
			write($"{new String('-', monthIndex + 1)} {DateTime.Now:HH:mm:ss:fff} {simulation.MonthLabel} {installmentCount}x after {installmentDelay} months:");
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
		simulation.ReInstallments = reInstallments[monthIndex];

		simulation.BalanceBR =
			config.SpentBR[monthIndex]
			+ config.NubankInstallments[monthIndex]
			+ config.C6Installments[monthIndex]
			+ reInstallments[monthIndex];

		var interest = 0f;

		if (simulation.BalanceBR > simulation.BalancePTBR)
		{
			simulation.ReInstallmentNeeded = simulation.BalanceBR - simulation.BalancePTBR;
			interest = config.Interests[installmentCount - 1][installmentDelay];
		}
		else
		{
			simulation.ReInstallmentNeeded = 0;
		}

		simulation.ReInstallmentTotal = (Single)Math.Ceiling(
			simulation.ReInstallmentNeeded * interest / installmentCount * 100
		) * installmentCount / 100;

		if (simulation.ReInstallmentTotal > simulation.Limit + config.Tolerance)
		{
			if (multiKey != null)
			{
				if (isTarget)
					write("WRONG");

				setWrong(multiKey);

				simulation.Valid = false;
				return simulation;
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
		simulation.BalancePTNext = (Single)Math.Round(balanceBRNext / config.Currency, 2);
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
			), nextMonthIndex, multiKey
		);
	}

	private async Task<ISimulation> oneOrAll(
		Func<Int16?, Int16?, Task<ISimulation>> execute,
		Int16 monthIndex, String? multiKey
	)
	{
		if (multiKey == null)
		{
			return await execute(null, null);
		}

		var firstSimulationTask = execute(1, 0);

		var firstSimulation = await firstSimulationTask;

		if (!firstSimulation.NeedReInstallment(monthIndex))
			return firstSimulation;

		var simulationTasks = new Task<ISimulation>[35];

		for (Int16 delay = 0; delay <= 2; delay++)
		{
			for (Int16 count = 1; count <= 12; count++)
			{
				var index = delay * 12 + count - 2;

				if (index < 0)
					continue;

				simulationTasks[index] = execute(count, delay);
			}
		}

		var simulations = new ISimulation[simulationTasks.Length+1];
		simulations[0] = firstSimulation;

		// ReSharper disable once ForCanBeConvertedToForeach because faster
		for (var st = 0; st < simulationTasks.Length; st++)
		{
			simulations[st+1] = await simulationTasks[st];
		}

		var lowestSimulation =
			simulations
				.Where(s => s.Valid)
				.MinBy(s => s.Total);

		if (lowestSimulation == null)
			setWrong(multiKey);

		return lowestSimulation ?? firstSimulation;
	}

	private static Int32 wrongKeysCount;
	private static void setWrong(String multiKey)
	{
		lock (multiKey)
		{
			File.AppendAllLines(multiWrongPath, new[] { multiKey });
			wrongKeysCount++;

			if (wrongKeysCount <= 100000)
				return;

			multiWrong = getWrongs();
			wrongKeysCount = 0;
		}
	}

	private static ISet<String> getWrongs()
	{
		if (File.Exists(multiWrongPath))
		{
			var lines = File.ReadAllLines(multiWrongPath)
				.OrderBy(l => l)
				.ToImmutableSortedSet();

			var nonRepeated = new SortedSet<String>();

			// ReSharper disable once ForCanBeConvertedToForeach because faster
			for (var l = 0; l < lines.Count; l++)
			{
				var line = lines[l];

				var parts = line.Split("_");
				var check = parts[0];
				var add = true;

				for (var p = 1; p < parts.Length; p++)
				{
					if (nonRepeated.Contains(check))
					{
						add = false;
						break;
					}

					check += "_" + parts[p];
				}

				if (add)
					nonRepeated.Add(line);
			}

			if (lines.Count > nonRepeated.Count)
				File.WriteAllLines(multiWrongPath, nonRepeated);

			return nonRepeated;
		}

		return new SortedSet<String>();
	}
}
