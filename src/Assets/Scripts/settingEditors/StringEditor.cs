namespace AssemblyCSharp
{
	using System;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;
	using UnityEngine.UI.Extensions;

	public class StringEditor : MonoBehaviour
	{
		[System.Serializable]
		public class ValueChangeEvent : UnityEvent<string>
		{
		}

		public delegate void ValueChangedEventHandler(StringEditor sender, string value);
		public event ValueChangedEventHandler ValueChanged = delegate { };
		public ValueChangeEvent onValueChanged;

		[SerializeField]
		private string Value = "";
		private NodeSetting setting;
		private Text settingNameText;
		private InputField input;

		void Awake()
		{
			foreach (Transform child in transform)
			{
				if (settingNameText == null)
					settingNameText = child.GetComponent<Text>();
				else if (input == null)
					input = child.GetComponent<InputField>();
			}
			Debug.Assert(input != null);

			input.text = Value;

			input.onValueChanged.AddListener(OnInputChanged);
		}

		void Start()
		{
			if (onValueChanged == null)
				onValueChanged = new ValueChangeEvent();
		}

		private void OnInputChanged(string text)
		{
			Value = text;
			ValueChanged(this, text);
			onValueChanged.Invoke(text);
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
				Value = (string)value.currentValue;
				setting = value;
			}
		}
	}
}