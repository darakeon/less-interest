namespace LessInterest;

public interface ISimulation
{
	ISimulation NewMonth();
	Int16 MonthIndex { get; }
	String MonthLabel { set; get; }
	Single NubankLimit { set; get; }
	Single NubankNewLimit { set; get; }
	Single C6Limit { set; get; }
	Single Limit { set; get; }
	Single BalancePTInitial { set; get; }
	Single BalancePTFinal { set; get; }
	Single BalancePTNext { set; get; }
	Single BalancePTBR { set; get; }
	Single ReInstallments { set; get; }
	Single BalanceBR { set; get; }
	Single ReInstallmentNeeded { set; get; }
	Single ReInstallmentAllowed { set; get; }
	Single ReInstallmentTotal { set; get; }
	Single ReInstallmentPart { set; get; }

	Boolean Valid { get; set; }

	Single Total { set; get; }

	Boolean NeedReInstallment(Int16 index);

	void Print(Action<String> write);

	Field[,] Transpose();

	Field this[Int16 row, Int16 column] { get; }
}
