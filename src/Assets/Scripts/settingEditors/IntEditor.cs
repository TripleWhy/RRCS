namespace AssemblyCSharp
{
	using UnityEngine.Events;

	public class IntEditor : NumberEditor<int>
	{
		[System.Serializable]
		public class ValueChangeEvent : UnityEvent<int>
		{
		}
		public ValueChangeEvent onValueChanged;

		public IntEditor()
		{
			ValueChanged += IntEditor_ValueChanged;
		}

		private void IntEditor_ValueChanged(NumberEditor<int> sender, int value)
		{
			if (onValueChanged != null)
				onValueChanged.Invoke(value);
		}

		protected override int Decrement(int value)
		{
			return value - 1;
		}

		protected override int Increment(int value)
		{
			return value + 1;
		}

		protected override int InitialMaxValue()
		{
			return int.MaxValue;
		}

		protected override int InitialMinValue()
		{
			return int.MinValue;
		}

		protected override int InitialValue()
		{
			return 0;
		}

		protected override bool TryParse(string text, out int result)
		{
			return int.TryParse(text, out result);
		}
	}
}
