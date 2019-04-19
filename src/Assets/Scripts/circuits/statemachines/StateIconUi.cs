using AssemblyCSharp;
using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp
{
	public class StateIconUi : MonoBehaviour
	{
		public Image image;
		private StateChip chip;

		private void Start()
		{
			chip = (StateChip) GetComponent<ChipUi>().Node;
		}

		private void Update()
		{
			if (chip.Active)
			{
				image.color = Color.white;
			}
			else
			{
				image.color = Color.gray;
			}
		}
	}
}