namespace AssemblyCSharp
{
	using System;

	public class MultiplexerChip : Chip
	{
		public MultiplexerChip(CircuitManager manager)
			: base(manager, 7, 1, true)
		{
		}

		override public int IconIndex
		{
			get
			{
				return 43;
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
			int signal = InInt(0);
			IConvertible value = InValue(1);

			outputPorts[0].Value = null;
			for (int i = 1; i < inputPortCount; i++)
			{
				if (Evaluate((NodeSetting.SelectorCondition)settings[i - 1].currentValue, signal))
				{
					outputPorts[0].Value = InValue(i);
					break;
				}
			}
		}

		override protected NodeSetting[] CreateSettings()
		{
			return new NodeSetting[]
			{
				NodeSetting.CreateSetting(NodeSetting.SettingType.SelectorCondition0),
				NodeSetting.CreateSetting(NodeSetting.SettingType.SelectorCondition1),
				NodeSetting.CreateSetting(NodeSetting.SettingType.SelectorCondition2),
				NodeSetting.CreateSetting(NodeSetting.SettingType.SelectorCondition3),
				NodeSetting.CreateSetting(NodeSetting.SettingType.SelectorCondition4),
				NodeSetting.CreateSetting(NodeSetting.SettingType.SelectorCondition5),
			};
		}
	}
}
