namespace AssemblyCSharp
{
	public class SimpleGreaterThanChip : SimpleCombarerChipBase
	{
		public SimpleGreaterThanChip(CircuitManager manager) : base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 4;
			}
		}

		override protected bool Compare(int a, int b)
		{
			return a > b;
		}
	}
}