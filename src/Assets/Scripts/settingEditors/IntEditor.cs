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

		[SerializeField]
		private int value = 0;
		[SerializeField]
		private int minimum = int.MinValue;
		[SerializeField]
		private int maximum = int.MaxValue;
		private NodeSetting setting;
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
				if (input != null)
				{
					input.text = val.ToString();
					plusButton.interactable = val < maximum;
					minusButton.interactable = val > minimum;
				}
				ValueChanged(this, val);
			}
		}

		public int Minimum
		{
			get
			{
				return minimum;
			}
			set
			{
				if (value == minimum)
					return;
				minimum = value;
				if (minusButton != null)
					minusButton.interactable = Value > minimum;
				if (Value < minimum)
					Value = minimum;
			}
		}


		public int Maximum
		{
			get
			{
				return maximum;
			}
			set
			{
				if (value == maximum)
					return;
				maximum = value;
				if (plusButton != null)
					plusButton.interactable = Value < maximum;
				if (Value > maximum)
					Value = maximum;
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