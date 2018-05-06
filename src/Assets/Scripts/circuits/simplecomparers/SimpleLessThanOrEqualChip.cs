namespace AssemblyCSharp
{
	public class SimpleLessThanOrEqualChip : SimpleCombarerChipBase
	{
		public SimpleLessThanOrEqualChip(CircuitManager manager) : base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 7;
			}
		}

		override protected bool Compare(int a, int b)
		{
			return a <= b;
		}
	}
}