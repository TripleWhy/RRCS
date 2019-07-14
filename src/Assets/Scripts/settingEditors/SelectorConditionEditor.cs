namespace AssemblyCSharp
{
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UI;

	public class SelectorConditionEditor : MonoBehaviour
	{
		[System.Serializable]
		public class ConditionChangeEvent : UnityEvent<NodeSetting.SelectorCondition>
		{
		}

		public delegate void ConditionChangedEventHandler(SelectorConditionEditor sender, NodeSetting.SelectorCondition condition);
		public event ConditionChangedEventHandler ConditionChanged = delegate { };
		public ConditionChangeEvent onConditionChanged;

		[SerializeField]
		private NodeSetting.SelectorCondition condition = new NodeSetting.SelectorCondition();
		private NodeSetting setting;
		private Text settingNameText;
		private Button operationButton;
		private Text operationButtonText;
		private InputField input;
		private Button plusButton;
		private Button minusButton;

		void Awake()
		{
			foreach (Transform child in transform)
			{
				if (settingNameText == null)
					settingNameText = child.GetComponent<Text>();
				else if (operationButton == null)
				{
					operationButton = child.GetComponent<Button>();
					operationButtonText = operationButton.GetComponentInChildren<Text>();
				}
				else if (input == null)
					input = child.GetComponent<InputField>();
				else if (minusButton == null)
					minusButton = child.GetComponent<Button>();
				else if (plusButton == null)
					plusButton = child.GetComponent<Button>();
			}
			DebugUtils.Assert(operationButton != null);
			DebugUtils.Assert(operationButtonText != null);
			DebugUtils.Assert(input != null);
			DebugUtils.Assert(plusButton != null);
			DebugUtils.Assert(minusButton != null);

			input.text = RhsArgument.ToString();

			operationButton.onClick.AddListener(OnOperationClicked);
			input.onValueChanged.AddListener(OnInputChanged);
			plusButton.onClick.AddListener(OnPlusClicked);
			minusButton.onClick.AddListener(OnMinusClicked);
		}

		void Start()
		{
			if (onConditionChanged == null)
				onConditionChanged = new ConditionChangeEvent();
		}

		private void OnOperationClicked()
		{
			switch (Operation)
			{
				case NodeSetting.SelectorCondition.LogicOperation.Equal:
					Operation = NodeSetting.SelectorCondition.LogicOperation.NotEqual;
					break;
				case NodeSetting.SelectorCondition.LogicOperation.NotEqual:
					Operation = NodeSetting.SelectorCondition.LogicOperation.LessThan;
					break;
				case NodeSetting.SelectorCondition.LogicOperation.LessThan:
					Operation = NodeSetting.SelectorCondition.LogicOperation.LessThanOrEqual;
					break;
				case NodeSetting.SelectorCondition.LogicOperation.LessThanOrEqual:
					Operation = NodeSetting.SelectorCondition.LogicOperation.GreaterThan;
					break;
				case NodeSetting.SelectorCondition.LogicOperation.GreaterThan:
					Operation = NodeSetting.SelectorCondition.LogicOperation.GreaterThanOrEqual;
					break;
				case NodeSetting.SelectorCondition.LogicOperation.GreaterThanOrEqual:
					Operation = NodeSetting.SelectorCondition.LogicOperation.Equal;
					break;
			}
		}

		private void OnPlusClicked()
		{
			RhsArgument++;
		}

		private void OnMinusClicked()
		{
			RhsArgument--;
		}

		private void OnInputChanged(string text)
		{
			int result;
			if (int.TryParse(text, out result))
				RhsArgument = result;
		}

		public NodeSetting.SelectorCondition Condition
		{
			get
			{
				return condition;
			}
			set
			{
				if (value == condition)
					return;
				condition = value;
				operationButtonText.text = condition.OperationChar.ToString();
				if (input != null)
					input.text = condition.rhsArgument.ToString();
				ConditionChanged(this, condition);
				onConditionChanged.Invoke(condition);
			}
		}

		public NodeSetting.SelectorCondition.LogicOperation Operation
		{
			get
			{
				return Condition.operation;
			}
			set
			{
				if (value == Condition.operation)
					return;
				Condition = new NodeSetting.SelectorCondition(value, Condition.rhsArgument);
			}
		}

		public int RhsArgument
		{
			get
			{
				return Condition.rhsArgument;
			}
			set
			{
				if (value == Condition.rhsArgument)
					return;
				Condition = new NodeSetting.SelectorCondition(Condition.operation, value);
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
				Condition = (NodeSetting.SelectorCondition)value.currentValue;
				setting = value;
			}
		}
	}
}