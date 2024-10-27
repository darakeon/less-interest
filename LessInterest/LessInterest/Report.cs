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
				Console.WriteLine(cell);
			}

			Console.WriteLine();
		}

		Console.WriteLine(total.Text);
	}

	public Field[,] Transpose()
	{
		var transposed = new Field[Height, Width];

		for (var r = 0; r < Width; r++)
		{
			for (var c = 0; c < Height; c++)
			{
				transposed[c, r] = this[r, c];
			}
		}

		return transposed;
	}

	public Int32 Width => table.Count;
	public Int32 Height => table.Max(s => s.Count);

	public Field this[Int32 row, Int32 column] => table[row][column];
}
