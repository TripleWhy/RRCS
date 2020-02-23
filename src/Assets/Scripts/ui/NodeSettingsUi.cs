namespace AssemblyCSharp
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	public class NodeSettingsUi : MonoBehaviour
	{
		public ValueEditor valueEditorPrefab;
		public SelectorConditionEditor selectorConditionEditorPrefab;
		public DataTypeEditor dataTypeEditorPrefab;

		private Text effectiveEvaluationIndexText;
		private IntEditor priorityEditor;
		private readonly List<NodeUi> selectedNodes = new List<NodeUi>();
		private readonly List<GameObject> editors = new List<GameObject>();

		private void Awake()
		{
			foreach (Transform child in transform)
			{
				if (priorityEditor == null)
					priorityEditor = child.GetComponent<IntEditor>();
				effectiveEvaluationIndexText = child.GetComponent<Text>() ?? effectiveEvaluationIndexText;
			}
			DebugUtils.Assert(priorityEditor != null);
			priorityEditor.ValueChanged += PriorityEditor_ValueChanged;
		}

		private void PriorityEditor_ValueChanged(NumberEditor<int> sender, int value)
		{
			DebugUtils.Assert(selectedNodes.Count == 1);
			RRCSManager.Instance.circuitManager.UpdateNodePriority(selectedNodes[0].Node, value);
		}

		private class SettingsTypeUsageData
		{
			public int usageCount;
			public Type valueType;
		}
		public void SetSelectedNodes(IEnumerable<NodeUi> nodes)
		{
			foreach (GameObject go in editors)
				Destroy(go);
			editors.Clear();

			if (selectedNodes.Count == 1)
				selectedNodes[0].Node.RingEvaluationPriorityChanged -= Node_RingEvaluationPriorityChanged;
			selectedNodes.Clear();
			foreach (NodeUi nodeUi in nodes)
				selectedNodes.Add(nodeUi);
			if (selectedNodes.Count == 0)
			{
				gameObject.SetActive(false);
				return;
			}
			gameObject.SetActive(true); //Executes Awake() the first time this happens. Therefore it has to be executed before anything else.

			if (selectedNodes.Count > 1)
			{
				priorityEditor.gameObject.SetActive(false);
				effectiveEvaluationIndexText.gameObject.SetActive(false);
			}
			else
			{
				priorityEditor.gameObject.SetActive(true);
				effectiveEvaluationIndexText.gameObject.SetActive(true);
				priorityEditor.Value = selectedNodes[0].Node.RingEvaluationPriority;
				effectiveEvaluationIndexText.text = EffectiveEvaluationIndexString(selectedNodes[0].Node.RingEvaluationPriority);
				selectedNodes[0].Node.RingEvaluationPriorityChanged += Node_RingEvaluationPriorityChanged;
			}

			Dictionary<NodeSetting.SettingType, SettingsTypeUsageData> typeUsages = new Dictionary<NodeSetting.SettingType, SettingsTypeUsageData>();
			foreach (NodeUi nodeUi in selectedNodes)
			{
				foreach (NodeSetting setting in nodeUi.Node.settings)
				{
					if (typeUsages.ContainsKey(setting.type))
					{
						SettingsTypeUsageData data = typeUsages[setting.type];
						data.usageCount++;
						if (data.valueType != null && data.valueType != setting.ValueType)
							data.valueType = null;
					}
					else
						typeUsages.Add(setting.type, new SettingsTypeUsageData { usageCount = 1, valueType = setting.ValueType });
				}
			}
			foreach (NodeSetting setting in selectedNodes[0].Node.settings)
			{
				{
					SettingsTypeUsageData data = typeUsages[setting.type];
					if (data.valueType == null)
						continue;
					if (data.usageCount != selectedNodes.Count)
						continue;
				}
				MonoBehaviour editor = CreateSettingEditor(setting);
				if (editor != null)
					editors.Add(editor.gameObject);
			}
		}

		private void Node_RingEvaluationPriorityChanged(CircuitNode source)
		{
			effectiveEvaluationIndexText.text = EffectiveEvaluationIndexString(source.RingEvaluationPriority);
		}

		//TODO this function should not exist, or simply return intex.ToString().
		private string EffectiveEvaluationIndexString(int index)
		{
			return "Effective Index: " + index;
		}

		private MonoBehaviour CreateSettingEditor(NodeSetting setting)
		{
			if (setting.ValueType == typeof(NodeSetting.SelectorCondition))
			{
				SelectorConditionEditor editor = Instantiate<SelectorConditionEditor>(selectorConditionEditorPrefab, transform);
				editor.Setting = setting;
				editor.ConditionChanged += Editor_ConditionChanged;
				return editor;
			}
			else if (setting.ValueType == typeof(NodeSetting.DataType))
			{
				DataTypeEditor editor = Instantiate(dataTypeEditorPrefab, transform);
				editor.Setting = setting;
				editor.DataTypeChanged += Editor_DataTypeChanged;
				return editor;
			}
			else
			{
				ValueEditor editor = Instantiate(valueEditorPrefab, transform);
				editor.Setting = setting;
				editor.ValueChanged += ValueEditor_ValueChanged;
				return editor;
			}
		}

		private void ValueEditor_ValueChanged(ValueEditor sender, object value)
		{
			foreach (NodeUi nodeUi in selectedNodes)
				nodeUi.Node.SetSetting(sender.Setting.type, value);
		}

		private void Editor_ConditionChanged(SelectorConditionEditor sender, NodeSetting.SelectorCondition value)
		{
			foreach (NodeUi nodeUi in selectedNodes)
				nodeUi.Node.SetSetting(sender.Setting.type, value);
		}

		private void Editor_DataTypeChanged(DataTypeEditor sender, NodeSetting.DataType value)
		{
			foreach (NodeUi nodeUi in selectedNodes)
				nodeUi.Node.SetSetting(sender.Setting.type, value);
		}
	}
}
