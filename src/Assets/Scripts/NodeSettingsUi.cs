namespace AssemblyCSharp
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	public class NodeSettingsUi : MonoBehaviour
	{
		public IntEditor intEditorPrefab;

		private IntEditor priorityEditor;
		private List<ChipUi> selectedChips = new List<ChipUi>();
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
			Debug.Assert(selectedChips.Count == 1);
			RRCSManager.Instance.circuitManager.UpdateNodePriority(selectedChips[0].Chip, value);
			priorityEditor.Value = selectedChips[0].Chip.RingEvaluationPriority;
		}

		public void SetSelectedChips(IEnumerable<ChipUi> chips)
		{
			foreach (GameObject go in editors)
				Destroy(go);
			editors.Clear();

			selectedChips.Clear();
			foreach (ChipUi chipUi in chips)
				selectedChips.Add(chipUi);
			if (selectedChips.Count == 0)
			{
				gameObject.SetActive(false);
				return;
			}
			gameObject.SetActive(true); //Executes Awake() the first time this happens. Therefore it has to be executed before anything else.

			if (selectedChips.Count > 1)
			{
				priorityEditor.gameObject.SetActive(false);
			}
			else
			{
				priorityEditor.gameObject.SetActive(true);
				priorityEditor.Value = selectedChips[0].Chip.RingEvaluationPriority;
			}

			Dictionary<NodeSetting.SettingType, int> typeUsages = new Dictionary<NodeSetting.SettingType, int>();
			foreach (ChipUi chipUi in selectedChips)
			{
				foreach (NodeSetting setting in chipUi.Chip.settings)
				{
					if (typeUsages.ContainsKey(setting.type))
						typeUsages[setting.type]++;
					else
						typeUsages.Add(setting.type, 1);
				}
			}
			foreach (NodeSetting setting in selectedChips[0].Chip.settings)
			{
				if (typeUsages[setting.type] != selectedChips.Count)
					continue;
				editors.Add(CreateSettingEditor(setting));
			}
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
			else
			{
				Debug.Assert(false);
				return null;
			}
		}

		private void IntEditor_ValueChanged(IntEditor sender, int value)
		{
			foreach (ChipUi chipUi in selectedChips)
				chipUi.Chip.SetSetting(sender.Setting.type, value);
		}
	}
}