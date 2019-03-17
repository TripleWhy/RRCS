namespace AssemblyCSharp
{
	public class StateChip : Chip
	{
		public StateChip(CircuitManager manager) : base(manager, 0, 3, false, StatePort.StatePortType.Node)
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
			for (int i = 0; i < outputPortCount; i++)
				outputPorts[i].Value = (int)settings[i].currentValue;
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
	}
}