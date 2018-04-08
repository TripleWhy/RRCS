namespace AssemblyCSharp
{
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.EventSystems;
	using System.Collections.Generic;
	using System;

	public class PortUi : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public bool isInput;
		public LineRenderer linePrefab;
		private Port port;

		private static Transform linesContainer;
		public RectTransform RectTransform { get; private set; }
		public Image Image { get; private set; }
		private LineRenderer draggingLine;
		private readonly Dictionary<PortUi, LineRenderer> connectedLines = new Dictionary<PortUi, LineRenderer>();
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

		void OnDestroy()
		{
			UiManager.Unregister(this);
		}

		public Port Port
		{
			get
			{
				return port;
			}
			set
			{
				if (port != null)
					throw new InvalidOperationException();
				port = value;
				UiManager.Register(this);
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

		private bool HasLines
		{
			get
			{
				return connectedLines.Count != 0;
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
			if (isInput && HasLines)
			{
				return;
			}
			else
			{
				draggingLine = Instantiate(linePrefab, linesContainer);
				draggingLine.SetPosition(LinePositionIndex, Center);
				draggingLine.startColor = draggingLine.endColor = Image.color;
			}
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
			if (dstPort == null || dstPort.isInput == isInput || dstPort.chipUi.IsSidebarChip || (isInput && HasLines) || (dstPort.isInput && dstPort.HasLines))
			{
				Destroy(draggingLine.gameObject);
				draggingLine = null;
			}
			else
				port.Connect(dstPort.port);
		}

		#endregion

		private Vector3 lastPos;
		void Update()
		{
			if (RectTransform.position != lastPos)
			{
				lastPos = RectTransform.position;
				foreach (LineRenderer line in connectedLines.Values)
					line.SetPosition(LinePositionIndex, Center);
			}
		}

		internal LineRenderer AddConnection(PortUi otherUi, LineRenderer line)
		{
			if (line == null)
			{
				if (draggingLine != null)
				{
					line = draggingLine;
					draggingLine = null;
				}
				else
					line = Instantiate(linePrefab, linesContainer);
			}
			if (LineUseStart)
				line.startColor = Image.color;
			else
				line.endColor = Image.color;
			connectedLines.Add(otherUi, line);
			line.SetPosition(LinePositionIndex, Center);
			return line;
		}

		internal bool RemoveConnection(PortUi otherUi, bool destroyLine)
		{
			LineRenderer line = connectedLines[otherUi];
			connectedLines.Remove(otherUi);
			if (line != null && object.ReferenceEquals(line, draggingLine))
			{
				if (destroyLine)
				{
					Destroy(line);
					draggingLine = null;
				}
				return false;
			}
			else
			{
				if (destroyLine)
				{
					Destroy(line);
					return false;
				}
				return true;
			}
		}
	}
}