﻿namespace LessInterest;

public class Program
{
	public static void Main(String[] args)
	{
		var config = Config.Init("config_2");

		var balancesPt = config.GenerateBalancesPT();
		var nubankLimit = config.NubankLimit;
		var c6Limit = config.C6Limit;

		var simulator = new Simulator(config, true);
		var simulation = simulator.ProcessAll(
			balancesPt, nubankLimit, c6Limit
		);

		if (simulation == null)
			return;
			
		var transposed = simulation.Transpose();

		for (var r = 0; r < transposed.GetLength(0); r++)
		{
			for (var c = 0; c < transposed.GetLength(1); c++)
			{
				Console.Write($"{transposed[r, c].Text} ");
			}
			Console.WriteLine();
		}

		Console.WriteLine(simulation.Total);
	}
}
