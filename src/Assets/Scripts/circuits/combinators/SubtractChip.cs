namespace AssemblyCSharp
{
	public class SubtractChip : SimpleCombinatorChip
	{
		public SubtractChip(CircuitManager manager) : base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 10;
			}
		}

		override protected int Combine(int a, int b)
		{
			return a - b;
		}
	}
}