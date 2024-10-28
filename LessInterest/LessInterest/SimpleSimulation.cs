using System.Reflection;

namespace LessInterest;

class SimpleSimulation : ISimulation
{
	public SimpleSimulation()
	{
		list = new List<SimpleSimulation>();
		list.Add(this);
	}

	private SimpleSimulation(IList<SimpleSimulation> list, Int32 parentMonthIndex)
	{
		list.Add(this);
		this.list = list;

		MonthIndex = parentMonthIndex + 1;
	}

	public ISimulation NewMonth()
	{
		var ascendingList = list.Take(MonthIndex+1).ToList();
		return new SimpleSimulation(ascendingList, MonthIndex);
	}

	private IList<SimpleSimulation> list { get; }

	public Int32 MonthIndex { get; }
	public String MonthLabel { get; set; } = "";

	public Decimal NubankInstallments { get; set; }
	public Decimal NubankLimit { get; set; }
	public Decimal C6Installments { get; set; }
	public Decimal C6Limit { get; set; }
	public Decimal Limit { get; set; }
	public Decimal Salary { get; set; }
	public Decimal SpentPT { get; set; }
	public Decimal BalancePTInitial { get; set; }
	public Decimal BalancePTFinal { get; set; }
	public Decimal BalancePTBR { get; set; }
	public Decimal SalaryBR { get; set; }
	public Decimal SpentBR { get; set; }
	public Decimal ReInstallments { get; set; }
	public Decimal BalanceBR { get; set; }
	public Decimal ReInstallmentNeeded { get; set; }
	public Decimal ReInstallmentTotal { get; set; }
	public Decimal ReInstallmentAllowed { get; set; }
	public Decimal ReInstallmentPart { get; set; }
	public Decimal BalancePTNext { get; set; }
	public Decimal NubankNewLimit { get; set; }

	public Boolean Valid { set; get; }
	public Decimal Total { get; set; }

	private static IDictionary<PropertyInfo, Func<Object, String>> exportable =>
		typeof(SimpleSimulation)
			.GetProperties()
			.Where(
				p => (
					p.PropertyType == typeof(Decimal)
					|| p.PropertyType == typeof(String)
				) && p.Name != nameof(Total)
			)
			.ToDictionary(
				p => p,
				toString
			);

	private static Func<Object, String> toString(PropertyInfo prop)
	{
		if (prop.PropertyType == typeof(Decimal))
			return (Object obj) => ((Decimal)prop.GetValue(obj)).ToString("0.00");

		return (Object obj) => (String)prop.GetValue(obj);
	}

	private static readonly Int32 height = exportable.Count;

	public Boolean NeedReInstallment(Int32 index)
	{
		return !Valid || list[index].ReInstallmentNeeded > 0;
	}

	public void Print(Action<String> write)
	{
		foreach (var simulation in list)
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
		var width = list.Count;
		var transposed = new Field[height, width];

		for (var r = 0; r < width; r++)
		{
			for (var c = 0; c < height; c++)
			{
				transposed[c, r] = this[r, c];
			}
		}

		return transposed;
	}

	public Field this[Int32 row, Int32 column]
	{
		get
		{
			var prop = exportable.Keys.ToList()[column];
			var value = exportable[prop];
			return new Field(prop.Name, value(list[row]));
		}
	}
}
