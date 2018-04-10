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
			return a % b;
		}
	}
}