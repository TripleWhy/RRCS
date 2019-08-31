namespace AssemblyCSharp
{
	public class IntEditor : NumberEditor<int>
	{
		protected override int Decrement(int value)
		{
			return value - 1;
		}

		protected override int Increment(int value)
		{
			return value + 1;
		}

		protected override int InitialMaxValue()
		{
			return int.MaxValue;
		}

		protected override int InitialMinValue()
		{
			return int.MinValue;
		}

		protected override int InitialValue()
		{
			return 0;
		}

		protected override bool TryParse(string text, out int result)
		{
			return int.TryParse(text, out result);
		}
	}
}
