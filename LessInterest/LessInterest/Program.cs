﻿namespace LessInterest;

public class Program
{
	public static void Main(String[] args)
	{
		var configFile = "config_2";
		var config = Config.Init(configFile);

		var balancesPt = config.GenerateBalancesPT();
		var nubankLimit = config.NubankLimit;
		var c6Limit = config.C6Limit;

		var simulator = new Simulator(config, Console.WriteLine);
		simulator.ProcessAll(balancesPt, nubankLimit, c6Limit, configFile);
	}
}
