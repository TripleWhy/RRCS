namespace AssemblyCSharp
{
    public class Connection
    {
        public Port sourcePort;
        public Port targetPort;

        public Connection(Port source, Port target)
        {
            sourcePort = source;
            targetPort = target;
        }

        public bool isConnectedToPort(Port port)
        {
            return port == sourcePort || port == targetPort;
        }
        
        public bool connectsSamePorts(StateMachineTransition transition)
        {
            return (this.sourcePort == transition.sourcePort) && (targetPort == transition.targetPort);
        }
        
        public virtual void Disconnect()
        {
            targetPort.Disconnect(this);
            sourcePort.Disconnect(this);
        }

        public Port getOtherPort(StatePort statePort)
        {
            if (statePort == sourcePort)
                return targetPort;
            return sourcePort;
        }
    }
}