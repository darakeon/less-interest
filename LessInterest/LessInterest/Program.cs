using Newtonsoft.Json;

namespace LessInterest
{
	internal class Program
	{
		public static void Main(String[] args)
		{
			//for row in table:
			//    for cel in row:
			//        if is instance(cel, float):
			//              simulation[m].Add($"{cel:0.00}", end=" ")
			//        else:
			//            simulation[m].Add(cel, end=" ")
			//    simulation[m].Add()

			var configJson = 
				File.ReadAllText("config.json")
					.Replace("_", "");

			var config = JsonConvert.DeserializeObject<Config>(configJson);

			process(config!, true);
		}

		private static List<List<String>> process(Config config, Boolean printSimulation)
		{
			var totalInterest = 0m;
			var reInstallments = new List<Decimal>();

			var simulation = new List<List<String>>();

			var currency = config.Currency;
			var balancesPt = config.BalancesPT;
			var months = config.Months;
			var salary = config.Salary;
			var spentPt = config.SpentPT;
			var spentBr = config.SpentBR;
			var nubankLimit = config.NubankLimit;
			var nubankInstallments = config.NubankInstallments;
			var c6Limit = config.C6Limit;
			var c6Installments = config.C6Installments;
			var interests = config.Interests;
			var initialInstallmentsCounts = config.InitialInstallmentsCounts;
			var initialInstallmentsDelays = config.InitialInstallmentsDelays;

			for (var m = 0; m < months.Count; m++)
			{
				var month = months[m];

				simulation.Add(new List<String>());

				if (c6Installments.Count <= m)
					c6Installments.Add(0);

				if (reInstallments.Count <= m)
					reInstallments.Add(0);

				simulation[m].Add($"month: {month}");

				simulation[m].Add($"nubank_installments: {nubankInstallments[m]}");
				nubankLimit += nubankInstallments[m];
				simulation[m].Add($"nubank_limit: {nubankLimit:0.00}");

				simulation[m].Add($"c6_installments: {c6Installments[m]}");
				c6Limit += c6Installments[m];
				simulation[m].Add($"c6_limit: {c6Limit}");

				var limit = nubankLimit + c6Limit;
				simulation[m].Add($"limit: {limit:0.00}");

				simulation[m].Add($"salary: {salary[m]}");
				simulation[m].Add($"spent_pt: {spentPt[m]}");

				var balancePt = balancesPt[m];

				simulation[m].Add($"balance_pt: {balancePt}");

				balancePt = balancePt + salary[m] - spentPt[m];
				simulation[m].Add($"balance_pt_left: {balancePt:0.00}");

				var balancePtBr = balancePt * currency;
				simulation[m].Add($"balance_pt_br: {balancePtBr:0.00}");

				simulation[m].Add($"salary_br: 0");
				simulation[m].Add($"spent_br: {spentBr[m]}");
				simulation[m].Add($"nubank_installments: {nubankInstallments[m]}");
				simulation[m].Add($"c6_installments: {c6Installments[m]}");
				simulation[m].Add($"reInstallments: {reInstallments[m]:0.00}");

				var balanceBr = spentBr[m] + nubankInstallments[m] + c6Installments[m] + reInstallments[m];
				simulation[m].Add($"balance_br: {balanceBr:0.00}");

				var reInstallment = 0m;
				var installmentCount = 1;
				var installmentDelay = 0;
				var interest = 0m;

				if (balanceBr > balancePtBr)
				{
					reInstallment = balanceBr - balancePtBr;

					installmentCount = initialInstallmentsCounts[m];
					installmentDelay = initialInstallmentsDelays[m];
					interest = interests[installmentCount - 1][installmentDelay];
				}

				simulation[m].Add($"re_installment: {reInstallment:0.00}");

				var reInstallmentTotal = Math.Ceiling(
						reInstallment * interest / installmentCount * 100
					) * installmentCount / 100;

				if (reInstallmentTotal > limit)
				{
					reInstallmentTotal = limit;
					reInstallment = reInstallmentTotal / interest;
				}

				totalInterest += (reInstallmentTotal - reInstallment);

				var reInstallmentPart = reInstallmentTotal / installmentCount;

				simulation[m].Add($"re_installment_total: {reInstallmentTotal:0.00}");
				simulation[m].Add($"re_installment_part: {reInstallmentPart:0.00}");

				balancePt = Math.Round((balancePtBr - balanceBr + reInstallment) / currency, 2);
				balancesPt.Add(balancePt);

				nubankLimit -= reInstallmentTotal;

				var nextReInstallment = m + 1 + installmentDelay;
				while (reInstallments.Count <= nextReInstallment + installmentCount)
				{
					reInstallments.Add(0);
				}

				for (var i = 0; i < installmentCount; i++)
				{
					reInstallments[i + nextReInstallment] += reInstallmentPart;
				}
			}

			simulation.Add(
				new List<String>
				{
					$"total_interest: {totalInterest:0.00}"
				}
			);

			if (printSimulation)
			{
				foreach (var month in simulation.Take(simulation.Count - 1))
				{
					foreach (var data in month)
					{
						Console.WriteLine(data);
					}

					Console.WriteLine();
				}

				Console.WriteLine(simulation[-1]);
			}

			return simulation;
		}
	}
}
