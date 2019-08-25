namespace AssemblyCSharp
{
	public class StateMachineTransition : Connection
	{
		public StatePort SourceStatePort { get; private set; }
		public StatePort TargetStatePort { get; private set; }
		public InputPort TransitionEnabledPort { get; private set; }

		public override Port SourcePort
		{
			get
			{
				return SourceStatePort;
			}
		}

		public override Port TargetPort
		{
			get
			{
				return TargetStatePort;
			}
		}

		public StateMachineTransition(StatePort source, StatePort target)
		{
			DebugUtils.Assert(!target.IsStateRootPort);
			SourceStatePort = source;
			TargetStatePort = target;
			if (!source.IsStateRootPort)
			{
				TransitionEnabledPort = new InputPort(target.node, false)
				{
					UnconnectedValue = 1
				};
			}
		}

		public override void Disconnect()
		{
			if (TransitionEnabledPort != null)
				TransitionEnabledPort.DisconnectAll();
			base.Disconnect();
		}

		public StatePort GetOtherPort(StatePort port)
		{
			if (object.ReferenceEquals(port, SourcePort))
				return TargetStatePort;
			return SourceStatePort;
		}
	}
}
