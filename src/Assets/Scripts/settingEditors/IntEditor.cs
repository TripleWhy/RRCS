namespace AssemblyCSharp
{
	using System;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;
	using UnityEngine.UI.Extensions;

	public class IntEditor : MonoBehaviour
	{
		[System.Serializable]
		public class ValueChangeEvent : UnityEvent<int>
		{
		}

		public delegate void ValueChangedEventHandler(IntEditor sender, int value);
		public event ValueChangedEventHandler ValueChanged = delegate { };
		public ValueChangeEvent onValueChanged;

		[SerializeField]
		private int currentValue = 0;
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

			plusButton.interactable = Value < maximum;
			minusButton.interactable = Value > minimum;
			input.text = Value.ToString();

			input.onValueChanged.AddListener(OnInputChanged);
			plusButton.onClick.AddListener(OnPlusClicked);
			minusButton.onClick.AddListener(OnMinusClicked);
		}

		void Start()
		{
			if (onValueChanged == null)
				onValueChanged = new ValueChangeEvent();
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
			int result;
			if (int.TryParse(text, out result))
				Value = result;
		}

		public int Value
		{
			get
			{
				return currentValue;
			}
			set
			{
				int val = Math.Min(Math.Max(minimum, value), maximum);
				if (val == this.currentValue)
					return;
				if (input != null)
				{
					input.text = val.ToString();
					plusButton.interactable = val < maximum;
					minusButton.interactable = val > minimum;
				}
				this.currentValue = val;
				ValueChanged(this, val);
				onValueChanged.Invoke(val);
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