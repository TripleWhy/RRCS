namespace AssemblyCSharp
{
	using UnityEngine;
	using UnityEngine.UI;

	public class StateChipUi : MonoBehaviour
	{
		public Text stateNameText;
		public Image stateIconImage;

		private void Start()
		{
			StateChip chip = (StateChip)GetComponent<ChipUi>().Node;
			chip.StateNameChanged += Chip_StateNameChanged;
			Chip_StateNameChanged(chip, chip.StateName);
			chip.StateActiveChanged += Chip_StateActiveChanged;
			Chip_StateActiveChanged(chip, chip.Active);
		}

		private void Chip_StateNameChanged(StateChip source, string stateName)
		{
			stateNameText.text = stateName;
		}

		private void Chip_StateActiveChanged(StateChip source, bool stateActive)
		{
			stateIconImage.color = stateActive ? Color.white : Color.gray;
		}
	}
}