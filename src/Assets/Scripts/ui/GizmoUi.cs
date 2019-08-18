using AssemblyCSharp.gizmos;

namespace AssemblyCSharp
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	public class GizmoUi : NodeUi
	{
		public enum GizmoType
		{
			None,
			Piston,
			Rotator,
			LookAt,
			Clamp
		};

		public GizmoType type;
		public PortUi in0;
		public Text gizmoValueText;

		[HideInInspector] [SerializeField] private bool skipSetup = false;

		private static Sprite[] icons;

		new void Awake()
		{
			base.Awake();
			if (icons == null)
				icons = Resources.LoadAll<Sprite>("iconsWhite");
		}


		void Start()
		{
			if (!skipSetup)
			{
				skipSetup = true;

				SetupPorts(InPortCount, Node.inputPorts, ref in0, ref inPorts);
			}
		}

		void Update()
		{
			Gizmo gizmo = (Gizmo) Node;
			if (!IsSidebarNode && gizmoValueText != null && gizmo != null)
			{
				gizmoValueText.text = gizmo.getGizmoValueString();

				if (type == GizmoType.Rotator)
				{
					if (((Rotator) gizmo).isMoveToTarget())
					{
						inPorts[2].RectTransform.localScale = new Vector3(1, 1, 1);
					}
					else
					{
						inPorts[2].RectTransform.localScale = new Vector3(0, 0, 0);
					}
				}


				if (type == GizmoType.Piston)
				{
					if (((Piston) gizmo).isMoveToTarget())
					{
						inPorts[2].RectTransform.localScale = new Vector3(1, 1, 1);
					}
					else
					{
						inPorts[2].RectTransform.localScale = new Vector3(0, 0, 0);
					}
				}
			}
		}

		private void SetupPorts(int portCount, Port[] ports, ref PortUi port0Ui, ref PortUi[] portUis)
		{
			DebugUtils.Assert(ports != null);
			DebugUtils.Assert(portUis == null);
			portUis = new PortUi[ports.Length];

			if (portCount <= 0)
				Destroy(port0Ui.gameObject);
			else
			{
				portUis[0] = port0Ui;
				port0Ui.nodeUi = this;
				port0Ui.Port = ports[0];
				RectTransform tr0 = port0Ui.RectTransform;


				Vector2 offset = new Vector2(0, tr0.sizeDelta.y * -3 / 2);
				for (int i = 1; i < portCount; i++)
				{
					PortUi portUi = Instantiate<PortUi>(port0Ui, transform);
					portUis[i] = portUi;
					portUi.nodeUi = this;
					portUi.Port = ports[i];
					portUi.RectTransform.anchoredPosition = tr0.anchoredPosition + i * offset;
					portUi.PortIndex = i;
				}
			}

			port0Ui = null;
		}

		protected override CircuitNode CreateNode(CircuitManager manager)
		{
			switch (type)
			{
				case GizmoType.Piston:
					return new Piston(manager);
				case GizmoType.Rotator:
					return new Rotator(manager);
				case GizmoType.LookAt:
					return new LookAt(manager);
				case GizmoType.Clamp:
					return new Clamp(manager);
				default:
					return null;
			}
		}

		protected override void OnMovedToWorld()
		{
			if (!skipSetup)
				return;
			DebugUtils.Assert(inPorts == null);
			inPorts = new PortUi[TotalInPortCount];
			outPorts = new PortUi[0];
			statePorts = new PortUi[0];
			int inPortIndex = 0;
			foreach (Transform child in transform)
			{
				PortUi port = child.GetComponent<PortUi>();
				if (port == null)
					continue;
				port.nodeUi = this;
				if (port.isInput)
				{
					int index = port.IsReset ? InPortCount : (inPortIndex++);
					DebugUtils.Assert(inPorts[index] == null);
					inPorts[index] = port;
					port.Port = Node.inputPorts[index];
				}
			}

			DebugUtils.Assert(!Array.Exists(inPorts, element => element == null));
		}

		public override string GetParams()
		{
			return type.ToString();
		}

		public override void ParseParams(string parameters)
		{
			DebugUtils.Assert(type == (GizmoType) Enum.Parse(typeof(GizmoType), parameters));
		}

		public bool TextActive
		{
			get { return (gizmoValueText != null) && gizmoValueText.gameObject.activeSelf; }
			set
			{
				if (gizmoValueText != null) gizmoValueText.gameObject.SetActive(value);
			}
		}

		public Gizmo Gizmo
		{
			get
			{
				return (Gizmo)Node;
			}
			protected set
			{
				Node = value;
			}
		}
	}
}
