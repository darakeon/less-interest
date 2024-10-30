namespace LessInterest;

public class Report
{
	protected IList<IList<Field>> table { get; } = new List<IList<Field>>();

	protected Field total { get; set; }

	public void Print(Action<String> write)
	{
		foreach (var row in table)
		{
			foreach (var cell in row)
			{
				write(cell.ToString());
			}

			write("");
		}

		write(total.Value ?? "");
	}

	public Field[,] Transpose()
	{
		var width = table.Count;
		var height = table.Max(s => s.Count);
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

	public Field this[Int16 row, Int16 column] => table[row][column];
}
