namespace AssemblyCSharp
{
	using System;

	public class AbsChip : Chip
	{
		public AbsChip(CircuitManager manager)
			: base(manager, 1, 1, true)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 46;
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
			IConvertible a = InValue(0);
			if (a is int)
				outputPorts[0].Value = Math.Abs((int)a);
			else if (a is long)
				outputPorts[0].Value = Math.Abs((long)a);
			else if (a is float)
				outputPorts[0].Value = Math.Abs((float)a);
			else if (a is double)
				outputPorts[0].Value = Math.Abs((double)a);
			else
				outputPorts[0].Value = InValue(0);
		}
	}
}
