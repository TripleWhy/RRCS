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
		};

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
			}
			Debug.Assert(false);
			return null;
		}
	}
}