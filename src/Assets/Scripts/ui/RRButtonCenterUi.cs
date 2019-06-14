using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace AssemblyCSharp
{

	public class RRButtonCenterUi : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		RRButtonUi buttonUi;

		void Start()
		{
			buttonUi = GetComponentInParent<RRButtonUi>();
			RaycastTarget = !buttonUi.IsSidebarNode;
		}

		public bool RaycastTarget
		{
			set
			{
				GetComponent<Image>().raycastTarget = value;
			}
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
		}

		public void OnDrag(PointerEventData eventData)
		{
		}

		public void OnEndDrag(PointerEventData eventData)
		{
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			RRCSManager.Instance.selectionManager.SelectionEnabled = false;
			buttonUi.button.Press();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			buttonUi.button.Release();
			RRCSManager.Instance.selectionManager.SelectionEnabled = true;
		}
	}
}