namespace AssemblyCSharp
{
	public class DoubleEditor : NumberEditor<double>
	{
		protected override double Decrement(double value)
		{
			return value - 0.1f;
		}

		protected override double Increment(double value)
		{
			return value + 0.1f;
		}

		protected override double InitialMaxValue()
		{
			return double.PositiveInfinity;
		}

		protected override double InitialMinValue()
		{
			return double.NegativeInfinity;
		}

		protected override double InitialValue()
		{
			return 0;
		}

		protected override bool TryParse(string text, out double result)
		{
			return double.TryParse(text, out result);
		}
	}
}
