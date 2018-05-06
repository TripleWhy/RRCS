namespace AssemblyCSharp
{
	public class AdvancedNotEqualsChip : AdvancedCombarerChipBase
	{
		public AdvancedNotEqualsChip(CircuitManager manager) : base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 3;
			}
		}

		override protected bool Compare(int a, int b)
		{
			return a != b;
		}
	}
}