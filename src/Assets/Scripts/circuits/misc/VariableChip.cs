namespace AssemblyCSharp
{
	using System;

	public class VariableChip : Chip
	{
		public VariableChip(CircuitManager manager)
			: base(manager, 0, 3, false)
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
			{
				DebugUtils.Assert(settings[i + 1].currentValue.GetType() == ((NodeSetting.DataType)settings[0].currentValue).SytemType);
				outputPorts[i].Value = (IConvertible)Convert.ChangeType(settings[i + 1].currentValue, ((NodeSetting.DataType)settings[0].currentValue).SytemType);
			}
		}

		override protected NodeSetting[] CreateSettings()
		{
			return new NodeSetting[]
			{
				NodeSetting.CreateSetting(NodeSetting.SettingType.NodeDataType),
				NodeSetting.CreateSetting(NodeSetting.SettingType.Output0),
				NodeSetting.CreateSetting(NodeSetting.SettingType.Output1),
				NodeSetting.CreateSetting(NodeSetting.SettingType.Output2),
			};
		}

		public override void SetSetting(NodeSetting setting, object value)
		{
			if (setting.type != NodeSetting.SettingType.NodeDataType)
				base.SetSetting(setting, value);
			else
			{
				if (value == setting.currentValue)
					return;
				setting.currentValue = value;

				Type newType = ((NodeSetting.DataType)value).SytemType;
				for (int i = 1; i < settings.Length; i++)
				{
					NodeSetting s = settings[i];
					s.valueType = newType;
					try
					{
						s.currentValue = Convert.ChangeType(s.currentValue, newType);
					}
					catch (FormatException)
					{
						s.currentValue = Activator.CreateInstance(newType);
					}
					DebugUtils.Assert(s.currentValue != null);
					DebugUtils.Assert(s.currentValue.GetType() == newType);
				}

				EmitEvaluationRequired();
			}
		}
	}
}
