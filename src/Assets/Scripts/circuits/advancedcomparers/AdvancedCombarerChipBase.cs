namespace AssemblyCSharp
{
	using System;

	public abstract class AdvancedCombarerChipBase : Chip
	{
		protected AdvancedCombarerChipBase(CircuitManager manager)
			: base(manager, 4, 2, true)
		{
		}

		protected override void EvaluateOutputs()
		{
			if (Compare(InValue(0), InValue(1)))
			{
				outputPorts[0].Value = InValue(2);
				outputPorts[1].Value = null;
			}
			else
			{
				outputPorts[0].Value = null;
				outputPorts[1].Value = InValue(3);
			}
		}

		abstract protected bool Compare(IConvertible a, IConvertible b);
	}
}

