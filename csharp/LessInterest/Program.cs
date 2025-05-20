namespace LessInterest;

public class Program
{
	public static void Main(String[] args)
	{
		var configFile = "config_5";
		var config = Config.Init(configFile);

		var balancesPt = config.GenerateBalancesPT();
		var nubankLimit = config.NubankLimit;
		var c6Limit = config.C6Limit;

		var simulator = new Simulator(config, Console.WriteLine);

		var multiKey = configFile.Replace("_", "");

		var task = simulator.ProcessAll(balancesPt, nubankLimit, c6Limit, multiKey);
		task.Wait();
	}
}
