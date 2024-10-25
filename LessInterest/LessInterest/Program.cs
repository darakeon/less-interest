using Newtonsoft.Json;

namespace LessInterest
{
	internal class Program
	{
		public static void Main(String[] args)
		{
			var configJson =
				File.ReadAllText("config.json")
					.Replace("_", "");

			var config = JsonConvert.DeserializeObject<Config>(configJson);

			var simulation = process(config!, false);
			var transposed = simulation.Transpose();

			for (var r = 0; r < transposed.GetLength(0); r++)
			{
				for (var c = 0; c < transposed.GetLength(1); c++)
				{
					Console.Write($"{transposed[r, c].Value} ");
				}
				Console.WriteLine();
			}
			
			foreach (var summary in simulation.summaries)
			{
				Console.WriteLine(summary);
			}
		}

		private static Report process(Config config, Boolean printSimulation)
		{
			var totalInterest = 0m;
			var reInstallments = new List<Decimal>();

			var simulation = new Report();

			var balancesPt = config.BalancesPT;
			var nubankLimit = config.NubankLimit;
			var c6Limit = config.C6Limit;
			var installmentsCounts = config.InitialInstallmentsCounts;
			var installmentsDelays = config.InitialInstallmentsDelays;

			for (var m = 0; m < config.Months.Count; m++)
			{
				var month = config.Months[m];

				if (reInstallments.Count <= m)
					reInstallments.Add(0);

				if (config.C6Installments.Count <= m)
					config.C6Installments.Add(0);

				if (config.NubankInstallments.Count <= m)
					config.NubankInstallments.Add(0);

				simulation.Add(m, new Field("month", month));

				simulation.Add(m, new Field("nubank_installments", config.NubankInstallments[m]));
				nubankLimit += config.NubankInstallments[m];
				simulation.Add(m, new Field("nubank_limit", nubankLimit));

				simulation.Add(m, new Field("c6_installments", config.C6Installments[m]));
				c6Limit += config.C6Installments[m];
				simulation.Add(m, new Field("c6_limit", c6Limit));

				var limit = nubankLimit + c6Limit;
				simulation.Add(m, new Field("limit", limit));

				simulation.Add(m, new Field("salary", config.Salary[m]));
				simulation.Add(m, new Field("spent_pt", config.SpentPT[m]));

				var balancePt = balancesPt[m];

				simulation.Add(m, new Field("balance_pt", balancePt));

				balancePt = balancePt + config.Salary[m] - config.SpentPT[m];
				simulation.Add(m, new Field("balance_pt_left", balancePt));

				var balancePtBr = balancePt * config.Currency;
				simulation.Add(m, new Field("balance_pt_br", balancePtBr));

				simulation.Add(m, new Field("salary_br", 0));
				simulation.Add(m, new Field("spent_br", config.SpentBR[m]));
				simulation.Add(m, new Field("nubank_installments", config.NubankInstallments[m]));
				simulation.Add(m, new Field("c6_installments", config.C6Installments[m]));
				simulation.Add(m, new Field("reInstallments", reInstallments[m]));

				var balanceBr =
					config.SpentBR[m]
					+ config.NubankInstallments[m]
					+ config.C6Installments[m]
					+ reInstallments[m];

				simulation.Add(m, new Field("balance_br", balanceBr));

				var reInstallment = 0m;
				var installmentCount = 1;
				var installmentDelay = 0;
				var interest = 0m;

				if (balanceBr > balancePtBr)
				{
					reInstallment = balanceBr - balancePtBr;

					installmentCount = installmentsCounts[m];
					installmentDelay = installmentsDelays[m];
					interest = config.Interests[installmentCount - 1][installmentDelay];
				}

				simulation.Add(m, new Field("re_installment", reInstallment));

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

				simulation.Add(m, new Field("re_installment_total", reInstallmentTotal));
				simulation.Add(m, new Field("re_installment_part", reInstallmentPart));

				balancePt = Math.Round((balancePtBr - balanceBr + reInstallment) / config.Currency, 2);
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
				new Field("total_interest", totalInterest)
			);

			if (printSimulation)
			{
				simulation.Print(Console.Write);
			}

			return simulation;
		}
	}
}
