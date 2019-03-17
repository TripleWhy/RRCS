namespace AssemblyCSharp
{
	public class StateMachineChip : Chip
	{
		public StateMachineChip(CircuitManager manager) : base(manager, 1, 5, true, StatePort.StatePortType.Root)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 14;
			}
		}

		override protected void EvaluateOutputs()
		{
			
		}
	}
}