using TMPro;

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
		public TextMeshProUGUI settingNameText;
		public Button tpyesButton;
		public TextMeshProUGUI typesButtonText;

		void Awake()
		{
			DebugUtils.Assert(tpyesButton != null);
			DebugUtils.Assert(typesButtonText != null);
			tpyesButton.onClick.AddListener(OnTypesClicked);
		}

		void Start()
		{
			if (onTypeChanged == null)
				onTypeChanged = new TypeChangedEvent();
		}

		private void OnTypesClicked()
		{
			switch (Type)
			{
				case NodeSetting.DataType.Type.Bool:
					Type = NodeSetting.DataType.Type.Int;
					break;
				case NodeSetting.DataType.Type.Int:
					Type = NodeSetting.DataType.Type.Long;
					break;
				case NodeSetting.DataType.Type.Long:
					Type = NodeSetting.DataType.Type.Float;
					break;
				case NodeSetting.DataType.Type.Float:
					Type = NodeSetting.DataType.Type.Double;
					break;
				case NodeSetting.DataType.Type.Double:
					Type = NodeSetting.DataType.Type.String;
					break;
				case NodeSetting.DataType.Type.String:
					Type = NodeSetting.DataType.Type.Bool;
					break;
			}
		}

		public NodeSetting.DataType DataType
		{
			get
			{
				return type;
			}
			set
			{
				if (value == type)
					return;
				type = value;
				typesButtonText.text = type.TypeText;
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
