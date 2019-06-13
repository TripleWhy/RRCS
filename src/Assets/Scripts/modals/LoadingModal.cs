using UnityEngine.UI;

namespace AssemblyCSharp.modals
{
    public class LoadingModal : Modal
    {
        public Text loadingText;

        public void Show(string message)
        {
            loadingText.text = message;
            Show();
        }
    }
}