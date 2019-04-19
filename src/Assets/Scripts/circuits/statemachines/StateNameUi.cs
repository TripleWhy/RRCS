using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp
{
	public class StateNameUi : MonoBehaviour
	{
		public Text text;
		private StateChip chip;

		private void Start()
		{
			chip = (StateChip) GetComponent<ChipUi>().Node;
		}

		private void Update()
		{
			text.text = (string) chip.settings[4].currentValue;
		}
	}
}