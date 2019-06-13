using UnityEngine;

namespace AssemblyCSharp.share
{
    public class Modal: MonoBehaviour
    {
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}