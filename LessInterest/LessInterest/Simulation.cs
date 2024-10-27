using System.Runtime.CompilerServices;

namespace LessInterest;

public class Simulation : Report
{
	private Int32 position = -1;

	public void NewMonth()
	{
		table.Add(new List<Field>());
		position++;
	}

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

	public Decimal ReInstallment
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

	private void add(String value, [CallerMemberName] String name = "")
	{
		table[position].Add(new Field(name, value));
	}

	private void add(Decimal value, [CallerMemberName] String name = "")
	{
		var field = get(name);

		if (field == null)
			table[position].Add(new Field(name, value));
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
		return table[position].FirstOrDefault(f => f.Name == name);
	}
}
