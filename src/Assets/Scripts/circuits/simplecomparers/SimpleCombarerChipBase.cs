namespace AssemblyCSharp
{
	using System;

	public abstract class SimpleCombarerChipBase : Chip
	{
		protected SimpleCombarerChipBase(CircuitManager manager)
			: base(manager, 2, 2, true)
		{
		}

		protected override Type ExpectedOutputType(int outputIndex)
		{
			return typeof(bool);
		}

		override protected void EvaluateOutputs()
		{
			IConvertible a = InValue(0);
			IConvertible b = InValue(1);
			bool result;
			if (a is string || b is string)
				result = Compare(ValueToString(a), ValueToString(b));
			else if (a is double || b is double)
				result = Compare(ValueToDouble(a), ValueToDouble(b));
			else if (a is float || b is float)
				result = Compare(ValueToFloat(a), ValueToFloat(b));
			else if (a is long || b is long)
				result = Compare(ValueToLong(a), ValueToLong(b));
			else if (a is int || b is int)
				result = Compare(ValueToInt(a), ValueToInt(b));
			else if (a is bool || b is bool)
				result = Compare(ValueToBool(a), ValueToBool(b));
			else
				result = Compare(a, b);

			outputPorts[0].Value = result;
			outputPorts[1].Value = !result;
		}

		abstract protected bool Compare(IConvertible a, IConvertible b);
	}
}

