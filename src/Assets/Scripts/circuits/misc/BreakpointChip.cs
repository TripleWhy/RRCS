namespace AssemblyCSharp
{
	using System;

	public class BreakpointChip : Chip
	{
		private bool lastIput = false;

		public BreakpointChip(CircuitManager manager) : base(manager, 1, 0, false)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 1;
			}
		}

		override protected void EvaluateOutputs()
		{
			bool input = InBool(0);
			if (input && !lastIput)
			{
				Manager.RequestSimulationPause();
			}
			lastIput = input;
		}
	}
}
