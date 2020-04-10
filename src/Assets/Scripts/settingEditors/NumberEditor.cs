using TMPro;

namespace AssemblyCSharp
{
	using System;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UI;

	public abstract class NumberEditor<T> : MonoBehaviour where T : IComparable<T>
	{
		public delegate void ValueChangedEventHandler(NumberEditor<T> sender, T value);
		public event ValueChangedEventHandler ValueChanged = delegate { };

		[SerializeField]
		private T currentValue;
		[SerializeField]
		private T minimum;
		[SerializeField]
		private T maximum;
		private NodeSetting setting;
		public TextMeshProUGUI settingNameText;
		public InputField input;
		public Button plusButton;
		public Button minusButton;

		protected abstract T Increment(T value);
		protected abstract T Decrement(T value);
		protected abstract T InitialValue();
		protected abstract T InitialMinValue();
		protected abstract T InitialMaxValue();
		protected abstract bool TryParse(string text, out T result);

		public NumberEditor()
		{
			currentValue = InitialValue();
			minimum = InitialMinValue();
			maximum = InitialMaxValue();
		}

		void Awake()
		{
			plusButton.interactable = Value.CompareTo(maximum) <= 0;
			minusButton.interactable = Value.CompareTo(minimum) >= 0;
			input.text = Value.ToString();

			input.onValueChanged.AddListener(OnInputChanged);
			plusButton.onClick.AddListener(OnPlusClicked);
			minusButton.onClick.AddListener(OnMinusClicked);
		}

		private void OnPlusClicked()
		{
			Value = Increment(Value);
		}

		private void OnMinusClicked()
		{
			Value = Decrement(Value);
		}

		private void OnInputChanged(string text)
		{
			T result;
			if (TryParse(text, out result))
				Value = result;
		}

		public T Value
		{
			get
			{
				return currentValue;
			}
			set
			{
				T val = value;
				if (val.CompareTo(minimum) < 0)
					val = minimum;
				if (val.CompareTo(maximum) > 0)
					val = maximum;
				if (val.CompareTo(this.currentValue) == 0)
					return;
				if (input != null)
				{
					input.text = val.ToString();
					plusButton.interactable = val.CompareTo(maximum) <= 0;
					minusButton.interactable = val.CompareTo(minimum) >= 0;
				}
				this.currentValue = val;
				ValueChanged(this, val);
			}
		}

		public T Minimum
		{
			get
			{
				return minimum;
			}
			set
			{
				minimum = value;
				if (minusButton != null)
					minusButton.interactable = Value.CompareTo(minimum) >= 0;
				if (Value.CompareTo(minimum) < 0)
					Value = minimum;
			}
		}


		public T Maximum
		{
			get
			{
				return maximum;
			}
			set
			{
				maximum = value;
				if (plusButton != null)
					plusButton.interactable = Value.CompareTo(maximum) <= 0;
				if (Value.CompareTo(maximum) > 0)
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
				Value = (T)value.currentValue;
				setting = value;
			}
		}
	}
}
