namespace AssemblyCSharp
{
	public class VariableChip : Chip
	{
		public VariableChip(CircuitManager manager) : base(manager, 0, 3, false)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 19;
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
				NodeSetting.CreateSetting(NodeSetting.SettingType.Output0),
				NodeSetting.CreateSetting(NodeSetting.SettingType.Output1),
				NodeSetting.CreateSetting(NodeSetting.SettingType.Output2),
			};
		}
	}
}