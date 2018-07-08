namespace AssemblyCSharp
{
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.EventSystems;
	using System.Collections.Generic;
	using System;

	public class PortUi : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public bool isInput;
		public Text valueText;
		public LineRenderer linePrefab;
		private Port port;

		private static Transform linesContainer;
		public RectTransform RectTransform { get; private set; }
		public Image Image { get; private set; }
		private LineRenderer draggingLine;
		private PortUi draggingOriginalConnectedPort;
		private readonly Dictionary<PortUi, LineRenderer> connectedLines = new Dictionary<PortUi, LineRenderer>();
		internal NodeUi nodeUi;

		private static readonly Color[] portColors =
			{
				new Color(0.83F, 0.20F, 0.20F, 1.00F),
				new Color(0.31F, 0.68F, 0.24F, 1.00F),
				new Color(0.12F, 0.45F, 0.81F, 1.00F),
				new Color(0.09F, 0.87F, 0.86F, 1.00F),
				new Color(0.91F, 0.29F, 0.50F, 1.00F),
				new Color(0.94F, 0.77F, 0.15F, 1.00F),
				new Color(0.06F, 0.15F, 0.18F, 1.00F),
			};

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
				{
					if (object.ReferenceEquals(port, value))
						return;
					throw new InvalidOperationException();
				}
				Debug.Assert(value.IsInput == isInput);
				Debug.Assert(value.isReset == IsReset);
				port = value;
				UiManager.Register(this);
			}
		}

		public bool IsReset
		{
			get
			{
				if (port != null)
					return port.isReset;
				// :-/
				return GetComponentsInChildren<Text>().Length == 1;
			}
		}

		public int PortIndex
		{
			set
			{
				Image.color = portColors[value % portColors.Length];
				foreach (LineRenderer line in connectedLines.Values)
				{
					if (LineUseStart)
						line.startColor = Image.color;
					else
						line.endColor = Image.color;
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

		private bool HasLines
		{
			get
			{
				return connectedLines.Count != 0;
			}
		}

		#region IPointerDownHandler implementation

		public void OnPointerDown(PointerEventData eventData)
		{
			RRCSManager.Instance.selectionManager.SelectionEnabled = false;
		}

		#endregion

		#region IPointerUpHandler implementation

		public void OnPointerUp(PointerEventData eventData)
		{
			RRCSManager.Instance.selectionManager.SelectionEnabled = true;
		}

		#endregion

		#region IBeginDragHandler implementation

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button != 0)
				return;
			if (draggingLine != null)
				return;
			if (nodeUi.IsSidebarNode)
				return;
			if (isInput && HasLines)
			{
				//connectedLines should contain exactly 1 entry.
				foreach (KeyValuePair<PortUi, LineRenderer> entry in connectedLines)
				{
					draggingOriginalConnectedPort = entry.Key;
					draggingLine = entry.Value;
				}
			}
			else
			{
				draggingLine = Instantiate(linePrefab, linesContainer);
				draggingLine.SetPosition(LinePositionIndex, Center);
				draggingLine.startColor = draggingLine.endColor = Image.color;
			}
			RRCSManager.Instance.selectionManager.SelectionEnabled = false;
		}

		#endregion

		#region IDragHandler implementation

		public void OnDrag(PointerEventData eventData)
		{
			if (draggingLine == null)
				return;
			Vector2 pos = eventData.pressEventCamera.ScreenToWorldPoint(eventData.position); //Cast Vector3 to Vector2 to discard the z coordinate
			if (draggingOriginalConnectedPort)
				draggingLine.SetPosition(LinePositionIndex, pos);
			else
				draggingLine.SetPosition(LinePositionOtherIndex, pos);
		}

		#endregion

		#region IEndDragHandler implementation

		public void OnEndDrag(PointerEventData eventData)
		{
			if (draggingLine == null)
				return;
			RRCSManager.Instance.selectionManager.SelectionEnabled = true;
			PortUi dstPort = null;
			foreach (GameObject go in eventData.hovered)
			{
				dstPort = go.GetComponent<PortUi>();
				if (dstPort != null)
					break;
			}
			if (draggingOriginalConnectedPort != null && object.ReferenceEquals(dstPort, this))
			{
				draggingOriginalConnectedPort = null;
				draggingLine.SetPosition(LinePositionIndex, Center);
				draggingLine = null;
				return;
			}
			PortUi srcPort = draggingOriginalConnectedPort ?? this;
			if (dstPort == null || dstPort.isInput == srcPort.isInput || dstPort.nodeUi.IsSidebarNode || (srcPort.isInput && srcPort.HasLines) || (dstPort.isInput && dstPort.HasLines))
			{
				if (draggingOriginalConnectedPort != null)
				{
					draggingLine = null;
					port.Disconnect(draggingOriginalConnectedPort.port);
					draggingOriginalConnectedPort = null;
				}
				else
				{
					Destroy(draggingLine.gameObject);
					draggingLine = null;
				}
			}
			else
			{
				if (draggingOriginalConnectedPort != null)
				{
					port.Disconnect(draggingOriginalConnectedPort.port);
					draggingOriginalConnectedPort = null;
					if (srcPort.draggingLine == null)
						srcPort.draggingLine = draggingLine;
					else
						Destroy(draggingLine.gameObject);
					draggingLine = null;
					srcPort.port.Connect(dstPort.port);
				}
				else
					srcPort.port.Connect(dstPort.port);
			}
		}

		#endregion

		private Vector3 lastPos;
		private int lastValue;
		void Update()
		{
			if (RectTransform.position != lastPos)
			{
				lastPos = RectTransform.position;
				foreach (LineRenderer line in connectedLines.Values)
					line.SetPosition(LinePositionIndex, Center);
			}
			if (valueText.gameObject.activeInHierarchy)
			{
				int value = port.GetValue();
				if (value != lastValue)
				{
					lastValue = value;
					valueText.text = value.ToString();
				}
			}
		}

		public bool TextActive
		{
			get
			{
				return valueText.gameObject.activeSelf;
			}
			set
			{
				valueText.gameObject.SetActive(value);
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
			if (line != null)
			{
				if (object.ReferenceEquals(line, draggingLine))
				{
					if (destroyLine)
					{
						Destroy(line.gameObject);
						draggingLine = null;
					}
					return false;
				}
				else
				{
					if (destroyLine)
					{
						Destroy(line.gameObject);
						return false;
					}
					return true;
				}
			}
			return true;
		}
	}
}