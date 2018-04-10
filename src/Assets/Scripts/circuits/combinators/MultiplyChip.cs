namespace AssemblyCSharp
{
	public class MultiplyChip : SimpleCombinatorChip
	{
		public MultiplyChip(CircuitManager manager) : base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 11;
			}
		}

		override protected int Combine(int a, int b)
		{
			return a * b;
		}
	}
}