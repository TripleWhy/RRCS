namespace AssemblyCSharp
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	public class PortUi : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public bool isInput;

		#region IPointerDownHandler implementation
		public void OnPointerDown(PointerEventData eventData)
		{
			print("PortUi.OnPointerDown");
		}
		#endregion

		#region IBeginDragHandler implementation

		public void OnBeginDrag(PointerEventData eventData)
		{
			print("PortUi.OnBeginDrag");
		}

		#endregion

		#region IDragHandler implementation

		public void OnDrag(PointerEventData eventData)
		{
			print("PortUi.OnDrag");
		}

		#endregion

		#region IEndDragHandler implementation

		public void OnEndDrag(PointerEventData eventData)
		{
			print("PortUi.OnEndDrag");
		}

		#endregion
	}
}