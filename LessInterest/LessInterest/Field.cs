namespace LessInterest;

public class Field
{
	private Field(String name)
	{
		Name = name;
	}

	public Field(String name, String text)
		: this(name)
	{
		Text = text;
	}

	public Field(String name, Decimal value)
		: this(name, $"{value:0.00}")
	{
		Value = value;
	}

	public String Name { get; }
	public String Text { get; }
	public Decimal Value { get; set; }

	public override String ToString()
	{
		return $"{Name}: {Text}";
	}
}
