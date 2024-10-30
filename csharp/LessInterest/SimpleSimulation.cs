using System.Reflection;

namespace LessInterest;

class SimpleSimulation : ISimulation
{
	public SimpleSimulation()
	{
		simulations = new[]{this};
	}

	private SimpleSimulation(SimpleSimulation[] parentSimulations, Int16 parentMonthIndex)
	{
		MonthIndex = (Int16)(parentMonthIndex + 1);

		simulations = parentSimulations
			.Take(MonthIndex)
			.Append(this)
			.ToArray();
	}

	public ISimulation NewMonth()
	{
		return new SimpleSimulation(simulations, MonthIndex);
	}

	private SimpleSimulation[] simulations { get; }

	public Int16 MonthIndex { get; }
	public String MonthLabel { get; set; } = "";

	public Single NubankLimit { get; set; }
	public Single C6Limit { get; set; }
	public Single Limit { get; set; }
	public Single BalancePTInitial { get; set; }
	public Single BalancePTFinal { get; set; }
	public Single BalancePTBR { get; set; }
	public Single ReInstallments { get; set; }
	public Single BalanceBR { get; set; }
	public Single ReInstallmentNeeded { get; set; }
	public Single ReInstallmentTotal { get; set; }
	public Single ReInstallmentAllowed { get; set; }
	public Single ReInstallmentPart { get; set; }
	public Single BalancePTNext { get; set; }
	public Single NubankNewLimit { get; set; }

	public Boolean Valid { set; get; }
	public Single Total { get; set; }

	private static IDictionary<PropertyInfo, Func<Object, String>> exportable =>
		typeof(SimpleSimulation)
			.GetProperties()
			.Where(
				p => (
					p.PropertyType == typeof(Single)
					|| p.PropertyType == typeof(String)
				) && p.Name != nameof(Total)
			)
			.ToDictionary(
				p => p,
				toString
			);

	private static Func<Object, String> toString(PropertyInfo prop)
	{
		if (prop.PropertyType == typeof(Single))
			return (Object obj) => ((Single)prop.GetValue(obj)).ToString("0.00");

		return (Object obj) => (String)prop.GetValue(obj);
	}

	private static readonly Int16 height = (Int16)exportable.Count;

	public Boolean NeedReInstallment(Int16 index)
	{
		return !Valid || simulations[index].ReInstallmentNeeded > 0;
	}

	public void Print(Action<String> write)
	{
		foreach (var simulation in simulations)
		{
			foreach (var prop in exportable)
			{
				write(prop.Value(simulation));
			}

			write("");
		}
	}

	public Field[,] Transpose()
	{
		var width = (Int16)simulations.Length;
		var transposed = new Field[height, width];

		for (Int16 r = 0; r < width; r++)
		{
			for (Int16 c = 0; c < height; c++)
			{
				transposed[c, r] = this[r, c];
			}
		}

		return transposed;
	}

	public Field this[Int16 row, Int16 column]
	{
		get
		{
			var prop = exportable.Keys.ToArray()[column];
			var value = exportable[prop];
			return new Field(prop.Name, value(simulations[row]));
		}
	}
}
