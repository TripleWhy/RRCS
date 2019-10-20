namespace AssemblyCSharp
{
	using System;
	using System.Text;

	public class Sign : CircuitNode
	{
		private class DisplayParameters
		{
			public string baseText;
			public bool limit;
			public IConvertible inputValueR;
			public IConvertible inputValueG;
			public IConvertible inputValueB;

			override public bool Equals(object obj)
			{
				return this == (obj as DisplayParameters);
			}

			public static bool operator ==(DisplayParameters a, DisplayParameters b)
			{
				if (object.ReferenceEquals(a, b))
					return true;
				if (object.ReferenceEquals(a, null) != object.ReferenceEquals(b, null))
					return false;
				return a.baseText == b.baseText
					&& a.limit == b.limit
					&& a.inputValueR == b.inputValueR
					&& a.inputValueG == b.inputValueG
					&& a.inputValueB == b.inputValueB;
			}

			public static bool operator !=(DisplayParameters a, DisplayParameters b)
			{
				return !(a == b);
			}
		}

		private const int limitLength = 20;
		public delegate void TextChangedEventHandler(string message, bool limitLength);

		private DisplayParameters lastDisplayParameters = new DisplayParameters();
		private string displayText;
		public event TextChangedEventHandler DisplayTextChanged = delegate { };

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

		public string DisplayText
		{
			get
			{
				if (displayText == null)
				{
					if (Manager == null)
						displayText = "RRCS";
					else
						EvaluateOutputs();
					DebugUtils.Assert(displayText != null);
				}
				return displayText;
			}
		}

		public bool DisplayTextIsLimited
		{
			get
			{
				return lastDisplayParameters.limit;
			}
		}

		protected override void EvaluateOutputs()
		{
			DisplayParameters displayParams = new DisplayParameters();
			displayParams.baseText = (string)settings[Math.Max(0, Math.Min(4, InInt(3)))].currentValue;
			displayParams.limit = (bool)settings[5].currentValue;
			displayParams.inputValueR = InValue(0);
			displayParams.inputValueG = InValue(1);
			displayParams.inputValueB = InValue(2);
			if (displayParams == lastDisplayParameters)
				return;

			StringBuilder message = new StringBuilder(displayParams.baseText);
			if (displayParams.limit && message.Length > limitLength)
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

			if (displayParams.limit)
				result = result.Substring(0, Math.Min(result.Length, limitLength));
			if (result == displayText && displayParams.limit == DisplayTextIsLimited)
				return;

			lastDisplayParameters = displayParams;
			displayText = result;
			DisplayTextChanged(result, displayParams.limit);
		}
	}
}
