namespace AssemblyCSharp
{
	public class SelectorChip : Chip
	{
		public SelectorChip(CircuitManager manager) : base(manager, 2, 7, true)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 14;
			}
		}

		private bool Evaluate(NodeSetting.SelectorCondition condition, int value)
		{
			switch (condition.operation)
			{
				case NodeSetting.SelectorCondition.LogicOperation.Equal:
					return value == condition.rhsArgument;
				case NodeSetting.SelectorCondition.LogicOperation.NotEqual:
					return value != condition.rhsArgument;
				case NodeSetting.SelectorCondition.LogicOperation.LessThan:
					return value < condition.rhsArgument;
				case NodeSetting.SelectorCondition.LogicOperation.LessThanOrEqual:
					return value <= condition.rhsArgument;
				case NodeSetting.SelectorCondition.LogicOperation.GreaterThan:
					return value > condition.rhsArgument;
				case NodeSetting.SelectorCondition.LogicOperation.GreaterThanOrEqual:
					return value >= condition.rhsArgument;
			}
			return false;
		}

		override protected void EvaluateOutputs()
		{
			bool firstOnly = (bool)settings[0].currentValue;
			int signal = InValue(0);
			int value = InValue(1);
			bool skip = false;
			for (int i = 0; i < outputPortCount; i++)
			{
				if (skip || !Evaluate((NodeSetting.SelectorCondition)settings[i + 1].currentValue, signal))
					outputPorts[i].Value = 0;
				else
				{
					skip = firstOnly;
					outputPorts[i].Value = value;
				}
			}
		}

		override protected NodeSetting[] CreateSettings()
		{
			return new NodeSetting[]
			{
				NodeSetting.CreateSetting(NodeSetting.SettingType.SelectorFirstOnly),
				NodeSetting.CreateSetting(NodeSetting.SettingType.SelectorCondition0),
				NodeSetting.CreateSetting(NodeSetting.SettingType.SelectorCondition1),
				NodeSetting.CreateSetting(NodeSetting.SettingType.SelectorCondition2),
				NodeSetting.CreateSetting(NodeSetting.SettingType.SelectorCondition3),
				NodeSetting.CreateSetting(NodeSetting.SettingType.SelectorCondition4),
				NodeSetting.CreateSetting(NodeSetting.SettingType.SelectorCondition5),
				NodeSetting.CreateSetting(NodeSetting.SettingType.SelectorCondition6),
			};
		}
	}
}