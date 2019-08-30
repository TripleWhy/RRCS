namespace AssemblyCSharp
{
	using System;
	using System.Text;

	public class Sign : CircuitNode
	{
		private const int limitLength = 20;
		public delegate void TextChangedEventHandler(string message, bool limitLength);

		public event TextChangedEventHandler TextChanged = delegate { };

		public Sign(CircuitManager manager) : base(manager, 4, 0, false)
		{
		}

		protected override NodeSetting[] CreateSettings()
		{
			return new NodeSetting[]
			{
				NodeSetting.CreateSetting(NodeSetting.SettingType.Message0),
				NodeSetting.CreateSetting(NodeSetting.SettingType.Message1),
				NodeSetting.CreateSetting(NodeSetting.SettingType.Message2),
				NodeSetting.CreateSetting(NodeSetting.SettingType.Message3),
				NodeSetting.CreateSetting(NodeSetting.SettingType.Message4),
				NodeSetting.CreateSetting(NodeSetting.SettingType.LimitMessage),
			};
		}

		protected override void EvaluateOutputs()
		{
			StringBuilder message = new StringBuilder((string)settings[Math.Max(0, Math.Min(4, InInt(3)))].currentValue);

			bool limit = (bool)settings[5].currentValue;
			if (limit && message.Length > limitLength)
				message.Remove(limitLength, message.Length - limitLength);

			message.Replace("{R}", "{0}");
			message.Replace("{G}", "{1}");
			message.Replace("{B}", "{2}");

			string result = message.ToString();
			try
			{
				UnityEngine.Debug.Log("format string: \"" + result + "\"");
				result = string.Format(result, InValue(0), InValue(1), InValue(2));
			}
			catch (IndexOutOfRangeException)
			{
			}
			catch (FormatException)
			{
			}

			if (limit)
				result = result.Substring(0, Math.Min(result.Length, limitLength));

			TextChanged(result, limit);
		}
	}
}
