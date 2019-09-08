namespace AssemblyCSharp
{
	using System;

	public class SignChip : Chip
	{
		public SignChip(CircuitManager manager)
			: base(manager, 1, 1, true)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 62;
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
				outputPorts[0].Value = Math.Sign((int)a);
			else if (a is long)
				outputPorts[0].Value = Math.Sign((long)a);
			else if (a is float)
				outputPorts[0].Value = Math.Sign((float)a);
			else if (a is double)
				outputPorts[0].Value = Math.Sign((double)a);
			else
				outputPorts[0].Value = null;
		}
	}
}
