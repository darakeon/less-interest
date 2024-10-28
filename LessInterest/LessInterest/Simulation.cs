using System.Runtime.CompilerServices;

namespace LessInterest;

public class Simulation : Report, ISimulation
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
		MonthIndex = (Int16)(original.MonthIndex + 1);
	}

	public ISimulation NewMonth()
	{
		return new Simulation(this);
	}

	public Int16 MonthIndex { get; }

	public String MonthLabel
	{
		set => add(value);
		get => getText();
	}

	public Single NubankInstallments
	{
		set => add(value);
		get => getValue();
	}

	public Single NubankLimit
	{
		set => add(value);
		get => getValue();
	}

	public Single NubankNewLimit
	{
		set => add(value);
		get => getValue();
	}

	public Single C6Installments
	{
		set => add(value);
		get => getValue();
	}

	public Single C6Limit
	{
		set => add(value);
		get => getValue();
	}

	public Single Limit
	{
		set => add(value);
		get => getValue();
	}

	public Single Salary
	{
		set => add(value);
		get => getValue();
	}

	public Single SpentPT
	{
		set => add(value);
		get => getValue();
	}

	public Single BalancePTInitial
	{
		set => add(value);
		get => getValue();
	}

	public Single BalancePTFinal
	{
		set => add(value);
		get => getValue();
	}

	public Single BalancePTNext
	{
		set => add(value);
		get => getValue();
	}

	public Single BalancePTBR
	{
		set => add(value);
		get => getValue();
	}

	public Single SalaryBR
	{
		set => add(value);
		get => getValue();
	}

	public Single SpentBR
	{
		set => add(value);
		get => getValue();
	}

	public Single ReInstallments
	{
		set => add(value);
		get => getValue();
	}

	public Single BalanceBR
	{
		set => add(value);
		get => getValue();
	}

	private Int16 reInstallmentNeededField;
	public Single ReInstallmentNeeded
	{
		set { reInstallmentNeededField = add(value); }
		get => getValue();
	}

	public Single ReInstallmentAllowed
	{
		set => add(value);
		get => getValue();
	}

	public Single ReInstallmentTotal
	{
		set => add(value);
		get => getValue();
	}

	public Single ReInstallmentPart
	{
		set => add(value);
		get => getValue();
	}

	public Boolean Valid { set; get; }

	public Single Total
	{
		set => total = create(value);
		get => total.Number ?? 0;
	}

	public Boolean NeedReInstallment(Int16 index)
	{
		return table[index][reInstallmentNeededField].Number > 0;
	}

	private void add(String value, [CallerMemberName] String name = "")
	{
		table[MonthIndex].Add(new Field(name, value));
	}

	private Int16 add(Single value, Int16 index = 0, [CallerMemberName] String name = "")
	{
		var field = get(name);

		if (field == null)
		{
			field = new Field(name, value);
			table[MonthIndex].Add(field);
		}
		else
		{
			field.Number = value;
		}

		return (Int16)table[MonthIndex].IndexOf(field);
	}

	private String getText([CallerMemberName] String name = "")
	{
		return get(name)?.Text ?? "";
	}

	private Single getValue([CallerMemberName] String name = "")
	{
		return get(name)?.Number ?? 0;
	}

	private Field? get([CallerMemberName] String name = "")
	{
		return table[MonthIndex].FirstOrDefault(f => f.Name == name);
	}

	private Field create(Single value, [CallerMemberName] String name = "")
	{
		return new Field(name, value);
	}
}
