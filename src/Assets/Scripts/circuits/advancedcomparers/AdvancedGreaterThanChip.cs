namespace AssemblyCSharp
{
	public class AdvancedGreaterThanChip : AdvancedCombarerChipBase
	{
		public AdvancedGreaterThanChip(CircuitManager manager) : base(manager)
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