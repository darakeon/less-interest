namespace LessInterest;

public class Field(String name, String value)
{
	public Field(String name, Decimal value)
		: this(name, $"{value:0.00}")
	{
	}

	public String Name { get; } = name;
	public String Value { get; } = value;

	public override String ToString()
	{
		return $"{Name}: {Value}";
	}
}