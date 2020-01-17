namespace AssemblyCSharp
{
	using System;

	public class MathConstantsChip : Chip
	{
		public MathConstantsChip(CircuitManager manager)
			: base(manager, 0, 2, false)
		{
			outputPorts[0].Value = Math.E;
			outputPorts[1].Value = Math.PI;
		}

		override public int IconIndex
		{
			get
			{
				return 66;
			}
		}

		protected override Type ExpectedOutputType(int outputIndex)
		{
			return typeof(double);
		}

		protected override void EvaluateImpl()
		{
		}

		protected override void EvaluateOutputs()
		{
		}
	}
}
