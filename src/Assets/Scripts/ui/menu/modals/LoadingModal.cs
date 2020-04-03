using TMPro;
using UnityEngine.UI;

namespace AssemblyCSharp.modals
{
    public class LoadingModal : Modal
    {
        public TextMeshProUGUI loadingText;

        public void Show(string message)
        {
            loadingText.text = message;
            Show();
        }
    }
}