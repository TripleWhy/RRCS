namespace AssemblyCSharp
{
	public class AdvancedLessThanChip : AdvancedCombarerChipBase
	{
		public AdvancedLessThanChip(CircuitManager manager) : base(manager)
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