using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp
{
	public class ImageToggle : MonoBehaviour
	{
		public Toggle toggle;

		public Sprite activeSprite;
		public Sprite inactiveSprite;

		public Image spriteContainer;

		void Start()
		{
			toggle.onValueChanged.AddListener(OnTargetToggleValueChanged);
		}

		void OnTargetToggleValueChanged(bool isActive)
		{
			if (spriteContainer != null)
			{
				if (isActive && activeSprite != null)
				{
					spriteContainer.sprite = activeSprite;
				}
				else if (inactiveSprite != null)
				{
					spriteContainer.sprite = inactiveSprite;
				}
			}
		}
	}
}