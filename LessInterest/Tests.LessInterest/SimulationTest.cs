using LessInterest;
using Newtonsoft.Json;

namespace Tests.LessInterest;

public class SimulationTest
{
	[SetUp]
	public void Setup()
	{			
		config = Config.Init();
		simulator = new Simulator(config, false);
	}

	public Config config { get; set; }
	public Simulator simulator { get; set; }

	[Test]
	public void InitialSimulation()
	{
		var balancesPt = config.BalancesPT;
		var nubankLimit = config.NubankLimit;
		var c6Limit = config.C6Limit;
		var installmentsCounts = config.InitialInstallmentsCounts;
		var installmentsDelays = config.InitialInstallmentsDelays;

		var simulation = simulator.Process(
			balancesPt, nubankLimit, c6Limit,
			installmentsCounts, installmentsDelays
		);

		var transposed = simulation.Transpose();

		var resultPath = Path.Combine("results", "result_1.json");
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