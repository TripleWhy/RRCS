namespace AssemblyCSharp
{
	public class AdvancedGreaterThanOrEqualChip : AdvancedCombarerChipBase
	{
		public AdvancedGreaterThanOrEqualChip(CircuitManager manager) : base(manager)
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