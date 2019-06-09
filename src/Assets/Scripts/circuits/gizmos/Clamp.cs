namespace AssemblyCSharp.gizmos
{
    public class Clamp : Gizmo
    {
        private int iconIndex;

        private bool isClamped = true;

        public Clamp(CircuitManager manager) : base(manager, 1)
        {
        }

        protected override void EvaluateOutputs()
        {
            isClamped = InValue(0) == 0;
        }

        public override string getGizmoValueString()
        {
            return isClamped ? "Clamped" : "Released";
        }
        
        public override void reset() { }
    }
}