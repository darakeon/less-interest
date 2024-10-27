using System.Runtime.CompilerServices;

namespace LessInterest;

public class Simulation : Report
{
	public Simulation()
	{
		table.Add(new List<Field>());
		MonthIndex = 0;
	}

	private Simulation(Simulation original)
	{
		foreach (var row in original.table)
		{
			table.Add(new List<Field>());
			foreach (var cell in row)
			{
				table.Last().Add(cell);
			}
		}

		table.Add(new List<Field>());
		MonthIndex = original.MonthIndex + 1;
	}

	public Simulation NewMonth()
	{
		return new Simulation(this);
	}

	public Int32 MonthIndex { get; }

	public String MonthLabel
	{
		set => add(value);
		get => getText();
	}

	public Decimal NubankInstallments
	{
		set => add(value);
		get => getValue();
	}

	public Decimal NubankLimit
	{
		set => add(value);
		get => getValue();
	}

	public Decimal NubankNewLimit
	{
		set => add(value);
		get => getValue();
	}

	public Decimal C6Installments
	{
		set => add(value);
		get => getValue();
	}

	public Decimal C6Limit
	{
		set => add(value);
		get => getValue();
	}

	public Decimal Limit
	{
		set => add(value);
		get => getValue();
	}

	public Decimal Salary
	{
		set => add(value);
		get => getValue();
	}

	public Decimal SpentPT
	{
		set => add(value);
		get => getValue();
	}

	public Decimal BalancePTInitial
	{
		set => add(value);
		get => getValue();
	}

	public Decimal BalancePTFinal
	{
		set => add(value);
		get => getValue();
	}

	public Decimal BalancePTNext
	{
		set => add(value);
		get => getValue();
	}

	public Decimal BalancePTBR
	{
		set => add(value);
		get => getValue();
	}

	public Decimal SalaryBR
	{
		set => add(value);
		get => getValue();
	}

	public Decimal SpentBR
	{
		set => add(value);
		get => getValue();
	}

	public Decimal ReInstallments
	{
		set => add(value);
		get => getValue();
	}

	public Decimal BalanceBR
	{
		set => add(value);
		get => getValue();
	}

	public Decimal ReInstallmentNeeded
	{
		set => add(value);
		get => getValue();
	}

	public Decimal ReInstallmentAllowed
	{
		set => add(value);
		get => getValue();
	}

	public Decimal ReInstallmentTotal
	{
		set => add(value);
		get => getValue();
	}

	public Decimal ReInstallmentPart
	{
		set => add(value);
		get => getValue();
	}

	public Decimal Total
	{
		set => total = create(value);
		get => total.Number ?? 0;
	}

	private void add(String value, [CallerMemberName] String name = "")
	{
		table[MonthIndex].Add(new Field(name, value));
	}

	private void add(Decimal value, [CallerMemberName] String name = "")
	{
		var field = get(name);

		if (field == null)
			table[MonthIndex].Add(new Field(name, value));
		else
			field.Number = value;
	}

	private String getText([CallerMemberName] String name = "")
	{
		return get(name)?.Text ?? "";
	}

	private Decimal getValue([CallerMemberName] String name = "")
	{
		return get(name)?.Number ?? 0;
	}

	private Field? get([CallerMemberName] String name = "")
	{
		return table[MonthIndex].FirstOrDefault(f => f.Name == name);
	}

	private Field create(Decimal value, [CallerMemberName] String name = "")
	{
		return new Field(name, value);
	}
}
