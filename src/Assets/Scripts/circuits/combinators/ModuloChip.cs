namespace AssemblyCSharp
{
	public class ModuloChip : SimpleCombinatorChip
	{
		public ModuloChip(CircuitManager manager) : base(manager)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 13;
			}
		}

		override protected int Combine(int a, int b)
		{
			if (b == 0)
				return 0;
			return a % b;
		}
	}
}