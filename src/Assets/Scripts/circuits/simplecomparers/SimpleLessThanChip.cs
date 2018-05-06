namespace AssemblyCSharp
{
	public class SimpleLessThanChip : SimpleCombarerChipBase
	{
		public SimpleLessThanChip(CircuitManager manager) : base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 6;
			}
		}

		override protected bool Compare(int a, int b)
		{
			return a < b;
		}
	}
}