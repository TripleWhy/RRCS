using UnityEngine;

namespace AssemblyCSharp.modals
{
	public class Modal : MonoBehaviour
	{
		public void Show()
		{
			gameObject.SetActive(true);
			ModalManager.RegisterModal(transform);
		}

		public void Hide()
		{
			gameObject.SetActive(false);
			ModalManager.UnregisterModal(transform);
		}
	}
}
