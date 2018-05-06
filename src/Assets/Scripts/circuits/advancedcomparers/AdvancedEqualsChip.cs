namespace AssemblyCSharp
{
	public class AdvancedEqualsChip : AdvancedCombarerChipBase
	{
		public AdvancedEqualsChip(CircuitManager manager) : base(manager)
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