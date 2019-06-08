namespace AssemblyCSharp
{
    public abstract class Gizmo : CircuitNode
    {
        public Gizmo(CircuitManager manager, int inputCount) :
            base(manager, inputCount, 0, false)
        {
        }

        public abstract string getGizmoValueString();
    }
}