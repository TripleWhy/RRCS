namespace AssemblyCSharp
{
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.EventSystems;

	public class PortUi : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public bool isInput;
		public LineRenderer linePrefab;

		private static Transform linesContainer;
		public RectTransform RectTransform { get; private set; }
		public Image Image { get; private set; }
		private LineRenderer draggingLine;
		internal ChipUi chipUi;

		void Awake()
		{
			RectTransform = (RectTransform)transform;
			Image = GetComponent<Image>();
			if (linesContainer == null)
			{
				foreach (GameObject go in gameObject.scene.GetRootGameObjects())
				{
					if (go.name == "Lines")
					{
						linesContainer = go.transform;
						break;
					}
				}
			}
		}

		private int LinePositionIndex
		{
			get
			{
				return isInput ? 0 : 1;
			}
		}

		private int LinePositionOtherIndex
		{
			get
			{
				return isInput ? 1 : 0;
			}
		}

		private Vector2 Center
		{
			get
			{
				return RectTransform.position;
			}
		}

		#region IPointerDownHandler implementation
		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button != 0)
				return;
			print("PortUi.OnPointerDown");
		}
		#endregion

		#region IBeginDragHandler implementation

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button != 0)
				return;
			if (draggingLine != null)
				return;
			if (chipUi.IsSidebarChip)
				return;
			draggingLine = Instantiate(linePrefab, linesContainer);
			int lpi = LinePositionIndex;
			draggingLine.SetPosition(lpi, Center);
			draggingLine.startColor = draggingLine.endColor = Image.color;
		}

		#endregion

		#region IDragHandler implementation

		public void OnDrag(PointerEventData eventData)
		{
			if (draggingLine == null)
				return;
			draggingLine.SetPosition(LinePositionOtherIndex, (Vector2)eventData.pressEventCamera.ScreenToWorldPoint(eventData.position));
		}

		#endregion

		#region IEndDragHandler implementation

		public void OnEndDrag(PointerEventData eventData)
		{
			if (draggingLine == null)
				return;
			print("PortUi.OnEndDrag");
			Destroy(draggingLine.gameObject);
			draggingLine = null;
		}

		#endregion
	}
}