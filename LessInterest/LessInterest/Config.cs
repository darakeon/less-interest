namespace LessInterest
{
	internal class Config
	{
		public Decimal Currency { get; set; }
		
		public IList<Decimal> BalancesPT { get; set; }
		
		public IList<String> Months { get; set; }
		
		public IList<Decimal> Salary { get; set; }

		public IList<Decimal> SpentPT { get; set; }
		public IList<Decimal> SpentBR { get; set; }

		public Decimal NubankLimit { get; set; }
		public IList<Decimal> NubankInstallments { get; set; }

		public Decimal C6Limit { get; set; }
		public IList<Decimal> C6Installments { get; set; }

		public IList<IList<Decimal>> Interests { get; set; }

		public IList<Int32> InitialInstallmentsCounts { get; set; }
		public IList<Int32> InitialInstallmentsDelays { get; set; }
	}
}
