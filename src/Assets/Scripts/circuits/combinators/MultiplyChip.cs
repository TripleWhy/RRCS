namespace AssemblyCSharp
{
	public class MultiplyChip : SimpleCombinatorChip
	{
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