namespace AssemblyCSharp
{
	using System;
	using System.Diagnostics;

	public class NodeSetting
	{
		public enum SettingType
		{
			Output0,
			Output1,
			Output2,
			SendPlayerId,
			LoopingTimer,
			SelectorFirstOnly,
			SelectorCondition0,
			SelectorCondition1,
			SelectorCondition2,
			SelectorCondition3,
			SelectorCondition4,
			SelectorCondition5,
			SelectorCondition6,
		};

		[System.Serializable]
		public class SelectorCondition
		{
			public enum LogicOperation
			{
				Equal, NotEqual, GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual,
			};

			public LogicOperation operation = LogicOperation.Equal;
			public int rhsArgument = 0;

			public SelectorCondition()
			{
			}

			public SelectorCondition(LogicOperation operation, int rhsArgument)
			{
				this.operation = operation;
				this.rhsArgument = rhsArgument;
			}
		}

		public readonly SettingType type;
		public readonly string displayName;
		public readonly Type valueType;
		public object currentValue;

		private NodeSetting(SettingType type, string displayName, Type valueType, object currentValue)
		{
			this.type = type;
			this.displayName = displayName;
			this.valueType = valueType;
			this.currentValue = currentValue;
		}

		public static NodeSetting CreateSetting(SettingType type)
		{
			switch (type)
			{
				case SettingType.Output0:
					return new NodeSetting(type, "R Signal", typeof(int), 0);
				case SettingType.Output1:
					return new NodeSetting(type, "G Signal", typeof(int), 0);
				case SettingType.Output2:
					return new NodeSetting(type, "B Signal", typeof(int), 0);
				case SettingType.SendPlayerId:
					return new NodeSetting(type, "Send Player ID", typeof(bool), false);
				case SettingType.LoopingTimer:
					return new NodeSetting(type, "Looping", typeof(bool), true);
				case SettingType.SelectorFirstOnly:
					return new NodeSetting(type, "First Only", typeof(bool), true);
				case SettingType.SelectorCondition0:
					return new NodeSetting(type, "R Condition", typeof(SelectorCondition), new SelectorCondition());
				case SettingType.SelectorCondition1:
					return new NodeSetting(type, "G Condition", typeof(SelectorCondition), new SelectorCondition());
				case SettingType.SelectorCondition2:
					return new NodeSetting(type, "B Condition", typeof(SelectorCondition), new SelectorCondition());
				case SettingType.SelectorCondition3:
					return new NodeSetting(type, "C Condition", typeof(SelectorCondition), new SelectorCondition());
				case SettingType.SelectorCondition4:
					return new NodeSetting(type, "M Condition", typeof(SelectorCondition), new SelectorCondition());
				case SettingType.SelectorCondition5:
					return new NodeSetting(type, "Y Condition", typeof(SelectorCondition), new SelectorCondition());
				case SettingType.SelectorCondition6:
					return new NodeSetting(type, "K Condition", typeof(SelectorCondition), new SelectorCondition());
			}
			Debug.Assert(false);
			return null;
		}
	}
}