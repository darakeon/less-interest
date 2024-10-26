﻿namespace LessInterest;

public class Program
{
	public static void Main(String[] args)
	{
		var config = Config.Init();

		var balancesPt = config.BalancesPT;
		var nubankLimit = config.NubankLimit;
		var c6Limit = config.C6Limit;
		var installmentsCounts = config.InitialInstallmentsCounts;
		var installmentsDelays = config.InitialInstallmentsDelays;

		var simulator = new Simulator(config, false);
		var simulation = simulator.Process(
			balancesPt, nubankLimit, c6Limit,
			installmentsCounts, installmentsDelays
		);
			
		var transposed = simulation.Transpose();

		for (var r = 0; r < transposed.GetLength(0); r++)
		{
			for (var c = 0; c < transposed.GetLength(1); c++)
			{
				Console.Write($"{transposed[r, c].Value} ");
			}
			Console.WriteLine();
		}

		Console.WriteLine(simulation.Total);
	}
}
