namespace AssemblyCSharp
{
    public abstract class Chip : CircuitNode
    {
        public Chip(CircuitManager manager, int inputCount, int outputCount, bool hasReset,
            StatePort.StatePortType statePortType = StatePort.StatePortType.None) :
            base(manager, inputCount, outputCount, hasReset, statePortType)
        {
        }

        public abstract int IconIndex { get; }
    }
}