namespace AssemblyCSharp
{
	public class DivideChip : SimpleCombinatorChip
	{
		public DivideChip(CircuitManager manager) : base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 12;
			}
		}

		override protected int Combine(int a, int b)
		{
			if (b == 0)
				return 0;
			return a / b;
		}
	}
}