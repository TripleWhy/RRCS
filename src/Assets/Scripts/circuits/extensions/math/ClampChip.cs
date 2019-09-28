namespace AssemblyCSharp
{
	using System;
	using System.Collections;

	public class ClampChip : Chip
	{
		public ClampChip(CircuitManager manager)
			: base(manager, 3, 1, true)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 51;
			}
		}

		protected override IConvertible DefaultInputValue(int inputIndex)
		{
			return 0;
		}

		protected override IConvertible DefaultOutputValue(int outputIndex)
		{
			return 0;
		}

		override protected void EvaluateOutputs()
		{
			IConvertible val = InValue(0);
			IConvertible min = InValue(1);
			IConvertible max = InValue(2);

			if (Comparer.DefaultInvariant.Compare(val, min) < 0)
				outputPorts[0].Value = min;
			else if (Comparer.DefaultInvariant.Compare(max, val) < 0)
				outputPorts[0].Value = max;
			else
				outputPorts[0].Value = val;
		}
	}
}
