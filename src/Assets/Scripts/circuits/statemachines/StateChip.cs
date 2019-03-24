using System.Collections.Generic;

namespace AssemblyCSharp
{
    public class StateChip : Chip
    {
        private bool isActive = false;
        public bool prevActive = false;

        public StateChip(CircuitManager manager) : base(manager, 0, 3, false, StatePort.StatePortType.Node)
        {
            EmitEvaluationRequired();
        }

        public override int IconIndex
        {
            get { return -1; }
        }

        public override void Evaluate()
        {
            EvaluateOutputs();

            prevActive = isActive;
        }

        protected override void EvaluateOutputs()
        {
            outputPorts[0].Value = outputPorts[1].Value = outputPorts[2].Value = 0;

            if (isActive)
            {
                outputPorts[1].Value = 1;
                if (!prevActive)
                    outputPorts[0].Value = 1;
            }
            else if (prevActive)
            {
                outputPorts[2].Value = 1;
            }
        }

        override protected NodeSetting[] CreateSettings()
        {
            return new NodeSetting[]
            {
                NodeSetting.CreateSetting(NodeSetting.SettingType.StateMinTimeInState),
                NodeSetting.CreateSetting(NodeSetting.SettingType.StateValue0),
                NodeSetting.CreateSetting(NodeSetting.SettingType.StateValue1),
                NodeSetting.CreateSetting(NodeSetting.SettingType.StateValue2),
                NodeSetting.CreateSetting(NodeSetting.SettingType.StateName),
            };
        }

        public bool Active
        {
            get { return isActive; }
            set
            {
                isActive = value;
                EmitEvaluationRequired();
            }
        }
        
        public override IEnumerable<CircuitNode> DependsOn()
        {
            var rootPorts = statePort.getAllConnectedRootPorts();
            foreach (var port in rootPorts)
            {
                yield return port.node;
            }
        }
    }
}