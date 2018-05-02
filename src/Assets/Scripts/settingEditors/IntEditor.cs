namespace AssemblyCSharp
{
	using System;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;
	using UnityEngine.UI.Extensions;

	public class IntEditor : MonoBehaviour
	{
		public delegate void ValueChangedEventHandler(IntEditor sender, int value);
		public event ValueChangedEventHandler ValueChanged = delegate { };

		private int value = 0;
		private NodeSetting setting;
		public int minimum = int.MinValue;
		public int maximum = int.MaxValue;
		private Text settingNameText;
		private InputField input;
		private Button plusButton;
		private Button minusButton;

		void Awake()
		{
			foreach (Transform child in transform)
			{
				if (settingNameText == null)
					settingNameText = child.GetComponent<Text>();
				else if (input == null)
					input = child.GetComponent<InputField>();
				else if (minusButton == null)
					minusButton = child.GetComponent<Button>();
				else if (plusButton == null)
					plusButton = child.GetComponent<Button>();
			}
			Debug.Assert(input != null);
			Debug.Assert(plusButton != null);
			Debug.Assert(minusButton != null);
			input.onValueChanged.AddListener(OnInputChanged);
			plusButton.onClick.AddListener(OnPlusClicked);
			minusButton.onClick.AddListener(OnMinusClicked);

			plusButton.interactable = value < maximum;
			minusButton.interactable = value > minimum;
		}

		private void OnPlusClicked()
		{
			Value++;
		}

		private void OnMinusClicked()
		{
			Value--;
		}

		private void OnInputChanged(string text)
		{
			Value = int.Parse(text);
		}

		public int Value
		{
			get
			{
				return value;
			}
			set
			{
				if (value == this.value)
					return;
				int val = Math.Min(Math.Max(minimum, value), maximum);
				if (val == this.value)
					return;
				this.value = val;
				input.text = val.ToString();
				plusButton.interactable = val < maximum;
				minusButton.interactable = val > minimum;
				ValueChanged(this, val);
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
				Value = (int)value.currentValue;
				setting = value;
			}
		}
	}
}