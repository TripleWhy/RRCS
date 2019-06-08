namespace AssemblyCSharp.gizmos
{
    public class LookAt : Gizmo
    {
        private int iconIndex;

        private bool isActive = false;

        public LookAt(CircuitManager manager) : base(manager, 1)
        {
        }

        protected override void EvaluateOutputs()
        {
            isActive = InValue(0) != 0;
        }

        override protected NodeSetting[] CreateSettings()
        {
            return new NodeSetting[]
            {
                NodeSetting.CreateSetting(NodeSetting.SettingType.TagToFollow)
            };
        }

        public override string getGizmoValueString()
        {
            return isActive ? "#" + settings[0].currentValue : "";
        }
    }
}