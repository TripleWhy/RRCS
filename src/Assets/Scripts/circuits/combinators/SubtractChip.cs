namespace AssemblyCSharp
{
	public class SubtractChip : SimpleCombinatorChip
	{
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