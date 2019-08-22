using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class StateMachineTransition : Connection
	{
		public InputPort transitionEnabledPort;

		public StateMachineTransition(StatePort source, StatePort target) : base(source, target)
		{
			DebugUtils.Assert(!target.isRootPort);
			sourcePort = source;
			targetPort = target;
			if (!source.isRootPort)
			{
				transitionEnabledPort = new InputPort(null, false);
				transitionEnabledPort.UnconnectedValue = 1;
			}
		}

		public override void Disconnect()
		{
			if (transitionEnabledPort != null)
				transitionEnabledPort.DisconnectAll();
			targetPort.Disconnect(this);
			sourcePort.Disconnect(this);
		}
	}
}