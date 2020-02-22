using UnityEngine;
using UnityEngine.UI;

namespace AssemblyCSharp.modals
{
	public class ShareSuccessModal : Modal
	{
		public InputField urlInput;
		public Button closeButton;

		public void Start()
		{
			closeButton.onClick.AddListener(Hide);
		}

		public void Show(string url)
		{
			urlInput.text = url;
			Show();
		}
	}
}
