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
		}

		private static Report process(Config config, Boolean printSimulation)
		{
			var totalInterest = 0m;
			var reInstallments = new List<Decimal>();

			var simulation = new Report();

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

				if (c6Installments.Count <= m)
					c6Installments.Add(0);

				if (reInstallments.Count <= m)
					reInstallments.Add(0);

				simulation.Add(m, new Field("month", month));

				simulation.Add(m, new Field("nubank_installments", nubankInstallments[m]));
				nubankLimit += nubankInstallments[m];
				simulation.Add(m, new Field("nubank_limit", nubankLimit));

				simulation.Add(m, new Field("c6_installments", c6Installments[m]));
				c6Limit += c6Installments[m];
				simulation.Add(m, new Field("c6_limit", c6Limit));

				var limit = nubankLimit + c6Limit;
				simulation.Add(m, new Field("limit", limit));

				simulation.Add(m, new Field("salary", salary[m]));
				simulation.Add(m, new Field("spent_pt", spentPt[m]));

				var balancePt = balancesPt[m];

				simulation.Add(m, new Field("balance_pt", balancePt));

				balancePt = balancePt + salary[m] - spentPt[m];
				simulation.Add(m, new Field("balance_pt_left", balancePt));

				var balancePtBr = balancePt * currency;
				simulation.Add(m, new Field("balance_pt_br", balancePtBr));

				simulation.Add(m, new Field("salary_br", 0));
				simulation.Add(m, new Field("spent_br", spentBr[m]));
				simulation.Add(m, new Field("nubank_installments", nubankInstallments[m]));
				simulation.Add(m, new Field("c6_installments", c6Installments[m]));
				simulation.Add(m, new Field("reInstallments", reInstallments[m]));

				var balanceBr = spentBr[m] + nubankInstallments[m] + c6Installments[m] + reInstallments[m];
				simulation.Add(m, new Field("balance_br", balanceBr));

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
