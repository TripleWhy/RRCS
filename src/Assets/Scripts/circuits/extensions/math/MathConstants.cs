namespace AssemblyCSharp
{
	using System;

	public class ConstantsChip : Chip
	{
		public ConstantsChip(CircuitManager manager)
			: base(manager, 0, 2, false)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 66;
			}
		}

		public override void Evaluate()
		{
			outputPorts[0].Value = Math.E;
			outputPorts[1].Value = Math.PI;
		}

		protected override void EvaluateOutputs()
		{
		}
	}
}
