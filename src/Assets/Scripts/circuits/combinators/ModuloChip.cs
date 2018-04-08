namespace AssemblyCSharp
{
	public class ModuloChip : SimpleCombinatorChip
	{
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