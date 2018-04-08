namespace AssemblyCSharp
{
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.EventSystems;
	using System.Collections.Generic;

	public class PortUi : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public bool isInput;
		public LineRenderer linePrefab;
		public Port port;

		private static Transform linesContainer;
		public RectTransform RectTransform { get; private set; }
		public Image Image { get; private set; }
		private LineRenderer draggingLine;
		private List<LineRenderer> connectedLines = new List<LineRenderer>();
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
				return LineUseStart ? 0 : 1;
			}
		}

		private int LinePositionOtherIndex
		{
			get
			{
				return LineUseStart ? 1 : 0;
			}
		}

		private bool LineUseStart
		{
			get
			{
				return isInput;
			}
		}

		private Vector2 Center
		{
			get
			{
				return RectTransform.position;
			}
		}

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
			PortUi dstPort = null;
			foreach (GameObject go in eventData.hovered)
			{
				dstPort = go.GetComponent<PortUi>();
				if (dstPort != null)
					break;
			}
			if (dstPort == null || dstPort.isInput == isInput || dstPort.chipUi.IsSidebarChip || (isInput && connectedLines.Count != 0) || (dstPort.isInput && dstPort.connectedLines.Count != 0))
				Destroy(draggingLine.gameObject);
			else
			{
				if (LineUseStart)
					draggingLine.endColor = dstPort.Image.color;
				else
					draggingLine.startColor = dstPort.Image.color;
				connectedLines.Add(draggingLine);
				dstPort.connectedLines.Add(draggingLine);
				draggingLine.SetPosition(LinePositionOtherIndex, dstPort.Center);
			}
			draggingLine = null;
		}

		#endregion

		private Vector3 lastPos;
		void Update()
		{
			if (RectTransform.position != lastPos)
			{
				lastPos = RectTransform.position;
				foreach (LineRenderer line in connectedLines)
					line.SetPosition(LinePositionIndex, Center);
			}
		}
	}
}