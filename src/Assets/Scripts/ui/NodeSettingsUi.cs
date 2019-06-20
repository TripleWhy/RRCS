namespace AssemblyCSharp
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	public class NodeSettingsUi : MonoBehaviour
	{
		public IntEditor intEditorPrefab;
		public BoolEditor boolEditorPrefab;
		public StringEditor stringEditorPrefab;
		public SelectorConditionEditor selectorConditionEditorPrefab;

		private IntEditor priorityEditor;
		private List<NodeUi> selectedNodes = new List<NodeUi>();
		private List<GameObject> editors = new List<GameObject>();

		private void Awake()
		{
			foreach (Transform child in transform)
			{
				if (priorityEditor == null)
					priorityEditor = child.GetComponent<IntEditor>();
			}
			Debug.Assert(priorityEditor != null);
			priorityEditor.ValueChanged += PriorityEditor_ValueChanged;
		}

		private void PriorityEditor_ValueChanged(IntEditor sender, int value)
		{
			Debug.Assert(selectedNodes.Count == 1);
			RRCSManager.Instance.circuitManager.UpdateNodePriority(selectedNodes[0].Node, value);
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
			}
			else
			{
				priorityEditor.gameObject.SetActive(true);
				priorityEditor.Value = selectedNodes[0].Node.RingEvaluationPriority;
				selectedNodes[0].Node.RingEvaluationPriorityChanged += Node_RingEvaluationPriorityChanged;
			}

			Dictionary<NodeSetting.SettingType, int> typeUsages = new Dictionary<NodeSetting.SettingType, int>();
			foreach (NodeUi nodeUi in selectedNodes)
			{
				foreach (NodeSetting setting in nodeUi.Node.settings)
				{
					if (typeUsages.ContainsKey(setting.type))
						typeUsages[setting.type]++;
					else
						typeUsages.Add(setting.type, 1);
				}
			}
			foreach (NodeSetting setting in selectedNodes[0].Node.settings)
			{
				if (typeUsages[setting.type] != selectedNodes.Count)
					continue;
				editors.Add(CreateSettingEditor(setting));
			}
		}

		private void Node_RingEvaluationPriorityChanged(CircuitNode source)
		{
			priorityEditor.Value = source.RingEvaluationPriority;
		}

		private GameObject CreateSettingEditor(NodeSetting setting)
		{
			if (setting.valueType == typeof(int))
			{
				IntEditor editor = Instantiate<IntEditor>(intEditorPrefab, transform);
				editor.Setting = setting;
				editor.ValueChanged += IntEditor_ValueChanged;
				return editor.gameObject;
			}
			else if (setting.valueType == typeof(bool))
			{
				BoolEditor editor = Instantiate<BoolEditor>(boolEditorPrefab, transform);
				editor.Setting = setting;
				editor.ValueChanged += BoolEditor_ValueChanged;
				return editor.gameObject;
			}
			else if (setting.valueType == typeof(string))
			{
				StringEditor editor = Instantiate<StringEditor>(stringEditorPrefab, transform);
				editor.Setting = setting;
				editor.ValueChanged += StringEditor_ValueChanged;
				return editor.gameObject;
			}
			else if (setting.valueType == typeof(NodeSetting.SelectorCondition))
			{
				SelectorConditionEditor editor = Instantiate<SelectorConditionEditor>(selectorConditionEditorPrefab, transform);
				editor.Setting = setting;
				editor.ConditionChanged += Editor_ConditionChanged;
				return editor.gameObject;
			}
			else
			{
				Debug.Assert(false);
				return null;
			}
		}

		private void IntEditor_ValueChanged(IntEditor sender, int value)
		{
			foreach (NodeUi nodeUi in selectedNodes)
				nodeUi.Node.SetSetting(sender.Setting.type, value);
		}

		private void BoolEditor_ValueChanged(BoolEditor sender, bool value)
		{
			foreach (NodeUi nodeUi in selectedNodes)
				nodeUi.Node.SetSetting(sender.Setting.type, value);
		}

		private void StringEditor_ValueChanged(StringEditor sender, string value)
		{
			foreach (NodeUi nodeUi in selectedNodes)
				nodeUi.Node.SetSetting(sender.Setting.type, value);
		}
		
		private void Editor_ConditionChanged(SelectorConditionEditor sender, NodeSetting.SelectorCondition value)
		{
			foreach (NodeUi nodeUi in selectedNodes)
				nodeUi.Node.SetSetting(sender.Setting.type, value);
		}
	}
}