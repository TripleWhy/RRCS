namespace AssemblyCSharp
{
	public class SimpleGreaterThanOrEqualChip : SimpleCombarerChipBase
	{
		public SimpleGreaterThanOrEqualChip(CircuitManager manager) : base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 5;
			}
		}

		override protected bool Compare(int a, int b)
		{
			return a >= b;
		}
	}
}