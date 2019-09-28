namespace AssemblyCSharp
{
	public class LongEditor : NumberEditor<long>
	{
		protected override long Decrement(long value)
		{
			return value - 1;
		}

		protected override long Increment(long value)
		{
			return value + 1;
		}

		protected override long InitialMaxValue()
		{
			return long.MaxValue;
		}

		protected override long InitialMinValue()
		{
			return long.MinValue;
		}

		protected override long InitialValue()
		{
			return 0;
		}

		protected override bool TryParse(string text, out long result)
		{
			return long.TryParse(text, out result);
		}
	}
}
