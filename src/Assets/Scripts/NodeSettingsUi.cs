namespace AssemblyCSharp
{
	using System.Collections.Generic;
	using UnityEngine;

	public class NodeSettingsUi : MonoBehaviour
	{
		private IntEditor priorityEditor;
		private List<ChipUi> selectedChips = new List<ChipUi>();

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
		}
	}
}