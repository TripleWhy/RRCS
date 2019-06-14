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
			ContinuousOutput,
			LoopingTimer,
			SelectorFirstOnly,
			SelectorCondition0,
			SelectorCondition1,
			SelectorCondition2,
			SelectorCondition3,
			SelectorCondition4,
			SelectorCondition5,
			SelectorCondition6,
			StateMinTimeInState,
			StateValue0,
			StateValue1,
			StateValue2,
			StateName,
			AccelerationTime,
			MoveToTarget,
			MaxTravelDistance,
			TagToFollow,
			Message0,
			Message1,
			Message2,
			Message3,
			Message4,
			LimitMessage,
			
		};

		[Serializable]
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

			public char OperationChar
			{
				get
				{
					switch (operation)
					{
						case LogicOperation.Equal:
							return '=';
						case LogicOperation.NotEqual:
							return '≠';
						case LogicOperation.GreaterThan:
							return '>';
						case LogicOperation.GreaterThanOrEqual:
							return '≥';
						case LogicOperation.LessThan:
							return '<';
						case LogicOperation.LessThanOrEqual:
							return '≤';
						default:
							throw new InvalidOperationException();
					}
				}
			}

			public override string ToString()
			{
				return OperationChar.ToString() + rhsArgument;
			}

			public static LogicOperation ParseOperation(char c)
			{
				switch (c)
				{
					case '=':
						return LogicOperation.Equal;
					case '≠':
						return LogicOperation.NotEqual;
					case '>':
						return LogicOperation.GreaterThan;
					case '≥':
						return LogicOperation.GreaterThanOrEqual;
					case '<':
						return LogicOperation.LessThan;
					case '≤':
						return LogicOperation.LessThanOrEqual;
					default:
						throw new FormatException("'" + c + "' is not a valid operation.");
				}
			}

			public static SelectorCondition Parse(string str)
			{
				return new SelectorCondition
				{
					operation = ParseOperation(str[0]),
					rhsArgument = int.Parse(str.Substring(1))
				};
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
				case SettingType.ContinuousOutput:
					return new NodeSetting(type, "Continuous Output", typeof(bool), true);
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
				case SettingType.StateMinTimeInState:
					return new NodeSetting(type, "Min Time In State (ticks)", typeof(int), 0);
				case SettingType.StateName:
					return new NodeSetting(type, "State Name", typeof(string), "State");
				case SettingType.StateValue0:
					return new NodeSetting(type, "Value 1", typeof(int), 0);
				case SettingType.StateValue1:
					return new NodeSetting(type, "Value 2", typeof(int), 0);
				case SettingType.StateValue2:
					return new NodeSetting(type, "Value 3", typeof(int), 0);
				case SettingType.AccelerationTime:
					return new NodeSetting(type, "Acceleration Time (ticks)", typeof(int), 0);
				case SettingType.MoveToTarget:
					return new NodeSetting(type, "Move To Target", typeof(bool), false);
				case SettingType.MaxTravelDistance:
					return new NodeSetting(type, "Max Travel Distance (dm)", typeof(int), 50);
				case SettingType.TagToFollow:
					return new NodeSetting(type, "Tag To Follow", typeof(string), "tag");
				case SettingType.Message0:
					return new NodeSetting(type, "Message 0:", typeof(string), "RRCS");
				case SettingType.Message1:
					return new NodeSetting(type, "Message 1:", typeof(string), "RRCS");
				case SettingType.Message2:
					return new NodeSetting(type, "Message 2:", typeof(string), "RRCS");
				case SettingType.Message3:
					return new NodeSetting(type, "Message 3:", typeof(string), "RRCS");
				case SettingType.Message4:
					return new NodeSetting(type, "Message 4:", typeof(string), "RRCS");
				case SettingType.LimitMessage:
					return new NodeSetting(type, "Limit Length", typeof(bool), true);
					
			}
			Debug.Assert(false);
			return null;
		}

		public void ParseValue(String stringValue)
		{
			if (valueType == typeof(int))
				currentValue = int.Parse(stringValue);
			else if (valueType == typeof(bool))
				currentValue = bool.Parse(stringValue);
			else if (valueType == typeof(string))
				currentValue = stringValue;
			else if (valueType == typeof(SelectorCondition))
				currentValue = SelectorCondition.Parse(stringValue);
			else
				throw new InvalidOperationException();
		}
	}
}