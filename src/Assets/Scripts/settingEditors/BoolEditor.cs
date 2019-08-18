namespace AssemblyCSharp
{
	using UnityEngine;
	using UnityEngine.UI;
	using Debug = System.Diagnostics.Debug;

	public class BoolEditor : MonoBehaviour
	{
		public delegate void ValueChangedEventHandler(BoolEditor sender, bool value);
		public event ValueChangedEventHandler ValueChanged = delegate { };

		private NodeSetting setting;
		private Text settingNameText;
		private Toggle toggle;

		void Awake()
		{
			toggle = GetComponent<Toggle>();
			settingNameText = GetComponentInChildren<Text>();
			DebugUtils.Assert(toggle != null);
			DebugUtils.Assert(settingNameText != null);
			toggle.onValueChanged.AddListener(OnToggleChanged);
		}

		private void OnToggleChanged(bool value)
		{
			ValueChanged(this, value);
		}

		public bool Value
		{
			get
			{
				return toggle.isOn;
			}
			set
			{
				if (value == Value)
					return;
				toggle.isOn = value;
			}
		}

		public NodeSetting Setting
		{
			get
			{
				return setting;
			}
			set
			{
				settingNameText.text = value.displayName;
				Value = (bool)value.currentValue;
				setting = value;
			}
		}
	}
}