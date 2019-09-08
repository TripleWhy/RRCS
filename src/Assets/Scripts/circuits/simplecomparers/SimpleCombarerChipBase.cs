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
			bool result = Compare(InValue(0), InValue(1));
			outputPorts[0].Value = result;
			outputPorts[1].Value = !result;
		}

		abstract protected bool Compare(IConvertible a, IConvertible b);
	}
}

