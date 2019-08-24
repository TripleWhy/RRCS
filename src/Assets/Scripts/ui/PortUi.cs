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
		public bool isState;
		public Text valueText;
		public Image delayIcon;
		public ConnectionUi connectionPrefab;
		private Port port;

		private static Transform linesContainer;
		public RectTransform RectTransform { get; private set; }
		public Image Image { get; private set; }
		private ConnectionUi draggingLine;
		private readonly List<ConnectionUi> connectedLines = new List<ConnectionUi>();
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
				linesContainer = GameObject.Find("WorldCanvas/Lines").transform;
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
				DebugUtils.Assert(value.IsDataInput == isInput);
				DebugUtils.Assert(value.IsResetPort == IsReset);
				port = value;
				UiManager.Register(this);
			}
		}

		public bool IsReset
		{
			get
			{
				if (port != null)
					return port.IsResetPort;
				// :-/
				return GetComponentsInChildren<Text>().Length == 1;
			}
		}

		public int PortIndex
		{
			set
			{
				Image.color = portColors[value % portColors.Length];
				foreach (ConnectionUi line in connectedLines)
				{
					line.UpdateColors();
				}
			}
		}

		public Vector2 Center
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
			if (nodeUi && nodeUi.IsSidebarNode)
				return;

			PortUi srcPort = this;
			if (isInput && HasLines)
			{
				//connectedLines should contain exactly 1 entry.
				ConnectionUi entry = connectedLines[0];
				entry.Disconnect();
				srcPort = entry.sourcePortUi;
			}

			draggingLine = Instantiate(connectionPrefab, linesContainer).GetComponent<ConnectionUi>();
			DebugUtils.Assert(draggingLine != null);

			draggingLine.sourcePortUi = srcPort;

			draggingLine.SetVirtualTargetPosition(Center);
			draggingLine.UpdateColors();

			RRCSManager.Instance.selectionManager.SelectionEnabled = false;
		}

		#endregion

		#region IDragHandler implementation

		public void OnDrag(PointerEventData eventData)
		{
			if (draggingLine == null)
				return;
			Vector2 pos =
				eventData.pressEventCamera
					.ScreenToWorldPoint(eventData.position); //Cast Vector3 to Vector2 to discard the z coordinate
			draggingLine.SetVirtualTargetPosition(pos);
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

			PortUi srcPort = draggingLine.sourcePortUi ?? this;

			draggingLine.Disconnect();
			draggingLine = null;

			if (dstPort != null && (dstPort.nodeUi == null || !dstPort.nodeUi.IsSidebarNode))
			{
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
				foreach (ConnectionUi connection in connectedLines)
					connection.UpdatePositions();
			}

			if (valueText != null && valueText.gameObject.activeInHierarchy)
			{
				int value = ((DataPort)Port).GetValue();
				if (value != lastValue)
				{
					lastValue = value;
					valueText.text = value.ToString();
				}
			}

			if (Port != null && delayIcon != null)
			{
				bool delayed;
				if (Port.IsConnected)
				{
					CircuitNode srcNode = Port.connections[0].SourcePort.node;
					CircuitNode dstNode = Port.node;
					delayed = srcNode.RingEvaluationPriority >= dstNode.RingEvaluationPriority;
				}
				else
					delayed = false;
				delayIcon.gameObject.SetActive(delayed);
			}
		}

		public bool TextActive
		{
			get
			{
				return (valueText != null) && valueText.gameObject.activeSelf;
			}
			set
			{
				if (valueText != null) valueText.gameObject.SetActive(value);
			}
		}

		internal ConnectionUi AddConnection(PortUi sourceUi, PortUi targetUi, Connection connection,
			ConnectionUi connectionUi)
		{
			if (connectionUi == null)
			{
				connectionUi = Instantiate(connectionPrefab, linesContainer).GetComponent<ConnectionUi>();

				connectionUi.sourcePortUi = sourceUi;
				connectionUi.targetPortUi = targetUi;
				connectionUi.Connection = connection;
			}

			connectedLines.Add(connectionUi);
			connectionUi.UpdatePositions();
			connectionUi.UpdateColors();
			return connectionUi;
		}

		internal bool RemoveConnection(ConnectionUi connection, bool destroyLine)
		{
			connectedLines.Remove(connection);
			if (connection != null)
			{
				if (destroyLine)
				{
					Destroy(connection.gameObject);
					return false;
				}

				return true;
			}

			return true;
		}
	}
}