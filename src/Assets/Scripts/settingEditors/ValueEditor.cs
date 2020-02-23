namespace AssemblyCSharp
{
	using System;
	using System.Reflection;
	using UnityEngine;

	public class ValueEditor : MonoBehaviour
	{
		public delegate void ValueChangedEventHandler(ValueEditor sender, object value);
		public event ValueChangedEventHandler ValueChanged = delegate { };

		public BoolEditor boolEditorPrefab;
		public IntEditor intEditorPrefab;
		public LongEditor longEditorPrefab;
		public FloatEditor floatEditorPrefab;
		public DoubleEditor doubleEditorPrefab;
		public StringEditor stringEditorPrefab;

		private NodeSetting setting;
		private MonoBehaviour editor;

		void OnDestroy()
		{
			if (!object.ReferenceEquals(setting, null))
				setting.ValueTypeChanged -= Setting_ValueTypeChanged;
		}

		private MonoBehaviour CreateSettingEditor(NodeSetting setting)
		{
			if (setting.ValueType == typeof(bool))
			{
				BoolEditor editor = Instantiate(boolEditorPrefab, transform);
				editor.Setting = setting;
				editor.ValueChanged += BoolEditor_ValueChanged;
				return editor;
			}
			else if (setting.ValueType == typeof(int))
			{
				IntEditor editor = Instantiate(intEditorPrefab, transform);
				editor.Setting = setting;
				editor.ValueChanged += NumberEditor_ValueChanged<int>;
				return editor;
			}
			else if (setting.ValueType == typeof(long))
			{
				LongEditor editor = Instantiate(longEditorPrefab, transform);
				editor.Setting = setting;
				editor.ValueChanged += NumberEditor_ValueChanged<long>;
				return editor;
			}
			else if (setting.ValueType == typeof(float))
			{
				FloatEditor editor = Instantiate(floatEditorPrefab, transform);
				editor.Setting = setting;
				editor.ValueChanged += NumberEditor_ValueChanged<float>;
				return editor;
			}
			else if (setting.ValueType == typeof(double))
			{
				DoubleEditor editor = Instantiate(doubleEditorPrefab, transform);
				editor.Setting = setting;
				editor.ValueChanged += NumberEditor_ValueChanged<double>;
				return editor;
			}
			else if (setting.ValueType == typeof(string))
			{
				StringEditor editor = Instantiate(stringEditorPrefab, transform);
				editor.Setting = setting;
				editor.ValueChanged += StringEditor_ValueChanged;
				return editor;
			}
			else
			{
				DebugUtils.Assert(false);
				return null;
			}
		}

		private void BoolEditor_ValueChanged(BoolEditor sender, bool value)
		{
			ValueChanged(this, value);
		}

		private void NumberEditor_ValueChanged<T>(NumberEditor<T> sender, T value) where T : IComparable<T>
		{
			ValueChanged(this, value);
		}

		private void StringEditor_ValueChanged(StringEditor sender, string value)
		{
			ValueChanged(this, value);
		}

		public object Value
		{
			get
			{
				return editor.GetType().GetProperty("Value").GetValue(editor, null);
			}
			set
			{
				PropertyInfo valueProperty = editor.GetType().GetProperty("Value");
				if (object.Equals(value, valueProperty.GetValue(editor, null)))
					return;
				valueProperty.SetValue(editor, value, null);
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
				if (object.Equals(value, setting))
					return;
				if (!object.ReferenceEquals(setting, null))
					setting.ValueTypeChanged -= Setting_ValueTypeChanged;
				setting = value;
				setting.ValueTypeChanged += Setting_ValueTypeChanged;
				UpdateEditor();
			}
		}

		private void Setting_ValueTypeChanged(NodeSetting sender, Type valueType)
		{
			UpdateEditor();
		}

		private void UpdateEditor()
		{
			if (!object.ReferenceEquals(editor, null))
			{
				editor.gameObject.SetActive(false);
				Destroy(editor.gameObject);
			}
			editor = CreateSettingEditor(Setting);
			RectTransform editorTransform = (RectTransform)editor.transform;
			RectTransform thisTransform = (RectTransform)transform;
			thisTransform.sizeDelta = editorTransform.sizeDelta;
		}
	}
}
