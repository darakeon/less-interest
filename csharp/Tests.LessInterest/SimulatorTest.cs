using LessInterest;
using Newtonsoft.Json;

namespace Tests.LessInterest;

public class SimulatorTest
{
	[SetUp]
	public void Setup() { }

	[Test]
	public void InitialSimulation()
	{
		testConfigAndResult(1);
	}

	[Test]
	public void Simulation20241027()
	{
		testConfigAndResult(2);
	}

	[Test]
	public void Simulation20241028()
	{
		testConfigAndResult(3);
	}

	[Test]
	public void Simulation20241103()
	{
		testConfigAndResult(4);
	}

	[Test]
	public void Simulation20241108()
	{
		testConfigAndResult(5);
	}

	private static void testConfigAndResult(Int32 id)
	{
		var config = Config.Init($"config_{id}");
		var simulator = new Simulator(config);

		var balancesPt = config.GenerateBalancesPT();
		var nubankLimit = config.NubankLimit;
		var c6Limit = config.C6Limit;
		var installmentsCounts = config.InitialInstallmentsCounts;
		var installmentsDelays = config.InitialInstallmentsDelays;

		var task = simulator.Process(
			balancesPt, nubankLimit, c6Limit,
			installmentsCounts, installmentsDelays
		);
		task.Wait();
		var simulation = task.Result;

		var transposed = simulation.Transpose();

		var resultPath = Path.Combine("results", $"result_{id}.json");
		var resultJson = File.ReadAllText(resultPath);
		var result = JsonConvert.DeserializeObject<String[,]>(resultJson)!;

		Assert.That(transposed.GetLength(0), Is.EqualTo(result.GetLength(0)));
		Assert.That(transposed.GetLength(1), Is.EqualTo(result.GetLength(1)));

		for (var r = 0; r < transposed.GetLength(0); r++)
		{
			for (var c = 0; c < transposed.GetLength(1); c++)
			{
				Assert.That(transposed[r, c].Value, Is.EqualTo(result[r, c]));
			}
		}
	}
}