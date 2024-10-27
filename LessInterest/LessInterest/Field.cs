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

	public Field(String name, Decimal number)
		: this(name)
	{
		Number = number;
	}

	public String Name { get; }
	public Decimal? Number { get; set; }
	public String? Text { get; set; }

	public String Value => Text ?? $"{Number:0.00}";

	public override String ToString()
	{
		return $"{Name}: {Value}";
	}
}
