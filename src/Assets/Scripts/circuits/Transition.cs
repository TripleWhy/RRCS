using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class StateMachineTransition: Connection
    {
        public InputPort transitionEnabledPort;
        
        public StateMachineTransition(StatePort source, StatePort target, InputPort transitionEnabled): base(source, target)
        {
            sourcePort = source;
            targetPort = target;
            transitionEnabledPort = transitionEnabled;
        }

        public override void Disconnect()
        {
            transitionEnabledPort.DisconnectAll();
            targetPort.connections.Remove(this);
            sourcePort.connections.Remove(this);
        }
    }
}