namespace AssemblyCSharp
{
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UI;

	public class DataTypeEditor : MonoBehaviour
	{
		[System.Serializable]
		public class TypeChangedEvent : UnityEvent<NodeSetting.DataType>
		{
		}

		public delegate void TypeChangedEventHandler(DataTypeEditor sender, NodeSetting.DataType type);
		public event TypeChangedEventHandler DataTypeChanged = delegate { };
		public TypeChangedEvent onTypeChanged;

		[SerializeField]
		private NodeSetting.DataType type = new NodeSetting.DataType();
		private NodeSetting setting;
		private Text settingNameText;
		private Dropdown typesDropdown;

		void Awake()
		{
			foreach (Transform child in transform)
			{
				if (settingNameText == null)
					settingNameText = child.GetComponent<Text>();
				else if (typesDropdown == null)
				{
					typesDropdown = child.GetComponent<Dropdown>();
				}
			}
			DebugUtils.Assert(typesDropdown != null);
			typesDropdown.onValueChanged.AddListener(OnTypeChanged);
		}

		void Start()
		{
			if (onTypeChanged == null)
				onTypeChanged = new TypeChangedEvent();
		}

		private void OnTypeChanged(int index)
		{
			Dropdown.OptionData option = typesDropdown.options[index];
			DataType = NodeSetting.DataType.Parse(option.text);
		}

		public NodeSetting.DataType DataType
		{
			get
			{
				return type;
			}
			set
			{
				if (value.Equals(type))
					return;
				type = value;

				string typeText = type.TypeText;
				for (int i = 0; i < typesDropdown.options.Count; i++)
				{
					if (typesDropdown.options[i].text == typeText)
					{
						typesDropdown.value = i;
						break;
					}
				}

				DataTypeChanged(this, type);
				onTypeChanged.Invoke(type);
			}
		}

		public NodeSetting.DataType.Type Type
		{
			get
			{
				return DataType.type;
			}
			set
			{
				if (value == DataType.type)
					return;
				DataType = new NodeSetting.DataType(value);
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
				DataType = (NodeSetting.DataType)value.currentValue;
				setting = value;
			}
		}
	}
}
