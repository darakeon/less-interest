namespace LessInterest;

public interface ISimulation
{
	ISimulation NewMonth();
	Int32 MonthIndex { get; }
	String MonthLabel { set; get; }
	Decimal NubankInstallments { set; get; }
	Decimal NubankLimit { set; get; }
	Decimal NubankNewLimit { set; get; }
	Decimal C6Installments { set; get; }
	Decimal C6Limit { set; get; }
	Decimal Limit { set; get; }
	Decimal Salary { set; get; }
	Decimal SpentPT { set; get; }
	Decimal BalancePTInitial { set; get; }
	Decimal BalancePTFinal { set; get; }
	Decimal BalancePTNext { set; get; }
	Decimal BalancePTBR { set; get; }
	Decimal SalaryBR { set; get; }
	Decimal SpentBR { set; get; }
	Decimal ReInstallments { set; get; }
	Decimal BalanceBR { set; get; }
	Decimal ReInstallmentNeeded { set; get; }
	Decimal ReInstallmentAllowed { set; get; }
	Decimal ReInstallmentTotal { set; get; }
	Decimal ReInstallmentPart { set; get; }
	Decimal Total { set; get; }
	void Print(Action<String> write);
	Field[,] Transpose();
	Field this[Int32 row, Int32 column] { get; }
}
