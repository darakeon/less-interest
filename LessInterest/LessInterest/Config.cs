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

	public IList<String> Months { get; set; }

	public IList<Single> Salary { get; set; }

	public IList<Single> SpentPT { get; set; }
	public IList<Single> SpentBR { get; set; }

	public Single NubankLimit { get; set; }
	public IList<Single> NubankInstallments { get; set; }

	public Single C6Limit { get; set; }
	public IList<Single> C6Installments { get; set; }

	public IList<IList<Single>> Interests { get; set; }

	public IList<Int16> InitialInstallmentsCounts { get; set; }
	public IList<Int16> InitialInstallmentsDelays { get; set; }

	public IList<Single> GenerateBalancesPT()
	{
		return new List<Single>
		{
			(Single)Math.Round(BalanceBR / Currency + BalancePT, 2)
		};
	}
}