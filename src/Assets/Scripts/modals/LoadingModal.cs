using UnityEngine.UI;

namespace AssemblyCSharp.share
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