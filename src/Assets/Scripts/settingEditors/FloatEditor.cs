namespace AssemblyCSharp
{
	public class FloatEditor : NumberEditor<float>
	{
		protected override float Decrement(float value)
		{
			return value - 0.1f;
		}

		protected override float Increment(float value)
		{
			return value + 0.1f;
		}

		protected override float InitialMaxValue()
		{
			return float.PositiveInfinity;
		}

		protected override float InitialMinValue()
		{
			return float.NegativeInfinity;
		}

		protected override float InitialValue()
		{
			return 0;
		}

		protected override bool TryParse(string text, out float result)
		{
			return float.TryParse(text, out result);
		}
	}
}
