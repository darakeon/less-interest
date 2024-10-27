namespace LessInterest;

public class Program
{
	public static void Main(String[] args)
	{
		var config = Config.Init("config_2");

		var balancesPt = config.GenerateBalancesPT();
		var nubankLimit = config.NubankLimit;
		var c6Limit = config.C6Limit;

		var simulator = new Simulator(config, Console.WriteLine);
		simulator.ProcessAll(balancesPt, nubankLimit, c6Limit);
	}
}
