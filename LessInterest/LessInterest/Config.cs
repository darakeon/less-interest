using Newtonsoft.Json;

namespace LessInterest;

public class Config
{
	public static Config Init(String filename)
	{
		var configPath = Path.Combine("configs", $"{filename}.json");

		var configJson =
			File.ReadAllText(configPath)
				.Replace("_", "");

		return JsonConvert.DeserializeObject<Config>(configJson)!;
	}

	public Single Currency { get; set; }

	public Single BalancePT { get; set; }
	public Single BalanceBR { get; set; }
	public Single Tolerance { get; set; }

	public String[] Months { get; set; }

	public Single[] Salary { get; set; }

	public Single[] SpentPT { get; set; }
	public Single[] SpentBR { get; set; }

	public Single NubankLimit { get; set; }
	public Single[] NubankInstallments { get; set; }

	public Single C6Limit { get; set; }
	public Single[] C6Installments { get; set; }

	public Single[,] Interests { get; set; }

	public Int16[] InitialInstallmentsCounts { get; set; }
	public Int16[] InitialInstallmentsDelays { get; set; }

	public Single[] GenerateBalancesPT()
	{
		var balancesPT = new Single[Months.Length+1];

		balancesPT[0] = (Single) Math.Round(BalanceBR / Currency + BalancePT, 2);

		return balancesPT;
	}
}