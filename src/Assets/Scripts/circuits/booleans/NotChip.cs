namespace AssemblyCSharp
{
	using System;

	public class NotChip : Chip
	{
		public NotChip(CircuitManager manager)
			: base(manager, 1, 1, true)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 17;
			}
		}

		protected override IConvertible DefaultInputValue(int inputIndex)
		{
			return true;
		}

		protected override IConvertible DefaultOutputValue(int outputIndex)
		{
			return false;
		}

		override protected void EvaluateOutputs()
		{
			outputPorts[0].Value = !InBool(0);
		}
	}
}