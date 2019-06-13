using System.Collections.Generic;
using UnityEngine;

namespace AssemblyCSharp.modals
{
    public static class ModalManager
    {
        private static HashSet<Transform> modals = new HashSet<Transform>();

        public static void RegisterModal(Transform modal)
        {
            modals.Add(modal);
            RRCSManager.Instance.selectionManager.gameObject.SetActive(false);
        }

        public static void UnregisterModal(Transform modal)
        {
            modals.Remove(modal);

            if (modals.Count == 0)
            {
                RRCSManager.Instance.selectionManager.gameObject.SetActive(true);
            }
        }
    }
}