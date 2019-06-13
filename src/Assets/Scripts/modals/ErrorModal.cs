using UnityEngine.UI;

namespace AssemblyCSharp.share
{
    public class ErrorModal : Modal
    {
        public Text errorTitle;
        public Text errorMessage;
        public Button closeButton;

        public void Show(string title, string message)
        {
            errorTitle.text = title;
            errorMessage.text = message;
            closeButton.onClick.AddListener(Hide);
            Show();
        }
    }
}