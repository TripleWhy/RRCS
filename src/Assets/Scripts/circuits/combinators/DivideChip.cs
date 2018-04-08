namespace AssemblyCSharp
{
	public class DivideChip : SimpleCombinatorChip
	{
		override public int IconIndex
		{
			get
			{
				return 12;
			}
		}

		override protected int Combine(int a, int b)
		{
			return a / b;
		}
	}
}