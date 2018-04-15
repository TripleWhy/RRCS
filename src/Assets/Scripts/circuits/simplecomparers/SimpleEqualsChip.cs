namespace AssemblyCSharp
{
	public class SimpleEqualsChip : SimpleCombarerChipBase
	{
		public SimpleEqualsChip(CircuitManager manager) : base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 2;
			}
		}

		override protected bool Compare(int a, int b)
		{
			return a == b;
		}
	}
}