namespace AssemblyCSharp
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.EventSystems;

	public class ChipUi : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		public enum ChipType
		{
			Add, Subtract, Multiply, Divide, Modulo,
			Equal, NotEqual, GreaterThan, TreaterThanOrEqual, LessThan, LessThanOrEqual,
			And, Or, Not,
			Variable,
			Output, Message,
			Delay, Timer,
			StartGame, GameState, SetScore, Score,
			SFX, Random,
			Respawn, PlayerHit,
		};

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

		public ChipType type;
		public PortUi in0;
		public PortUi inR;
		public PortUi out0;
		public PortUi outR;
		public Image icon;

		public Chip Chip { get; private set; }
		[HideInInspector]
		[SerializeField]
		private bool skipSetup = false;

		private PortUi[] inPorts;
		private PortUi[] outPorts;

		private Vector3 pointerWorldOffset;
		private RectTransform rectTransform;
		private Canvas canvas;
		private static Canvas worldCanvas;
		private static Sprite[] icons;
		private RectTransform canvasRectTransform;
		private ChipUi draggingInstance;

		public bool IsSidebarChip{ get; private set; }

		void Awake()
		{
			Chip = CreateChip();
			UiManager.Register(this);
		}

		void Start()
		{
			if (worldCanvas == null)
			{
				foreach (Canvas c in FindObjectsOfType<Canvas>())
				{
					if (c.tag == "WorldUi")
					{
						worldCanvas = c;
						break;
					}
				}
				icons = Resources.LoadAll<Sprite>("iconsWhite");
			}
			rectTransform = (RectTransform)transform;
			canvas = GetComponentInParent<Canvas>();
			canvasRectTransform = (RectTransform)canvas.transform;
			IsSidebarChip = (canvas.tag == "Sidebar");

			if (!skipSetup)
			{
				skipSetup = true;
				int maxTotalPortCount = Mathf.Max(TotalInPortCount, TotalOutPortCount);
				bool useCompactFormat = false;
				if (maxTotalPortCount >= 4)
				{
					useCompactFormat = true;
					rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, ((3 * maxTotalPortCount - 1) * in0.RectTransform.sizeDelta.y) / 2);
				}
				SetupPorts(InPortCount, Chip.inputPorts, useCompactFormat, ref in0, ref inR, ref inPorts);
				SetupPorts(OutPortCount, Chip.outputPorts, useCompactFormat, ref out0, ref outR, ref outPorts);
				Sprite sprite = GetSprite();
				if (sprite != null)
					icon.sprite = sprite;
			}
		}

		void OnDestroy()
		{
			UiManager.Unregister(this);
		}

		private void SetupPorts(int portCount, Port[] ports, bool useCompactFormat, ref PortUi port0UI, ref PortUi resetPortUi, ref PortUi[] portUIs)
		{
			portUIs = new PortUi[ports.Length];

			if (portCount <= 0)
				Destroy(port0UI.gameObject);
			else
			{
				portUIs[0] = port0UI;
				port0UI.chipUi = this;
				port0UI.Port = ports[0];
				RectTransform tr0 = port0UI.RectTransform;
				
				if (useCompactFormat)
				{
					tr0.anchoredPosition = new Vector2(tr0.anchoredPosition.x, -tr0.sizeDelta.y / 2);
					((RectTransform)resetPortUi.transform).anchoredPosition = new Vector2(0, tr0.sizeDelta.y / 2);
				}
				
				Vector2 offset = new Vector2(0, tr0.sizeDelta.y * -3 / 2);
				for (int i = 1; i < portCount; i++)
				{
					PortUi portUi = Instantiate<PortUi>(port0UI, transform);
					portUIs[i] = portUi;
					portUi.chipUi = this;
					portUi.Port = ports[i];
					portUi.RectTransform.anchoredPosition = tr0.anchoredPosition + i * offset;
					portUi.Image.color = portColors[i % portColors.Length];
				}
			}
			if (HasReset)
			{
				portUIs[portCount] = resetPortUi;
				resetPortUi.chipUi = this;
				resetPortUi.Port = ports[portCount];
			}
			else
				Destroy(resetPortUi.gameObject);
			port0UI = null;
			resetPortUi = null;
		}

		private Chip CreateChip()
		{
			return CreateChip(type);
		}

		private static Chip CreateChip(ChipType type)
		{
			switch(type)
			{
				case ChipType.Add:
					return new AddChip();
				case ChipType.Subtract:
					return new SubtractChip();
				case ChipType.Multiply:
					return new MultiplyChip();
				case ChipType.Divide:
					return new DivideChip();
				case ChipType.Modulo:
					return new ModuloChip();
				case ChipType.And:
					return new AndChip();
				case ChipType.Or:
					return new OrChip();
				case ChipType.Not:
					return new NotChip();
				default:
					return null;
			}
		}

		private Sprite GetSprite()
		{
			return icons[Chip.IconIndex];
		}

		public int InPortCount
		{
			get
			{
				return Chip.inputPortCount;
			}
		}

		public int TotalInPortCount
		{
			get
			{
				return Chip.inputPorts.Length;
			}
		}

		public int OutPortCount
		{
			get
			{
				return Chip.outputPortCount;
			}
		}

		public int TotalOutPortCount
		{
			get
			{
				return Chip.outputPorts.Length;
			}
		}

		public bool HasReset
		{
			get
			{
				return Chip.hasReset;
			}
		}

		#region IPointerDownHandler implementation

		public void OnPointerDown(PointerEventData eventData)
		{
			if (eventData.button != 0)
				return;
			Vector2 n_originalLocalPointerPosition;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out n_originalLocalPointerPosition);
			pointerWorldOffset = rectTransform.InverseTransformPoint(n_originalLocalPointerPosition) - rectTransform.InverseTransformPoint(new Vector3());
		}

		#endregion

		#region IBeginDragHandler implementation

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button != 0)
				return;
			if (draggingInstance != null)
				return;
			if (!IsSidebarChip)
				return;
			draggingInstance = Instantiate(this, canvasRectTransform);
			draggingInstance.GetComponent<Image>().raycastTarget = false;
		}

		#endregion

		#region IDragHandler implementation

		public void OnDrag(PointerEventData eventData)
		{
			if (eventData.button != 0)
				return;
			ChipUi chip = draggingInstance ?? this;

			if (chip.rectTransform == null || chip.canvasRectTransform == null)
				return;

			Vector2 worldPosition = eventData.position;
			if (eventData.pressEventCamera != null)
				worldPosition = eventData.pressEventCamera.ScreenToWorldPoint(eventData.position);

			chip.rectTransform.position = (Vector3)(worldPosition) - pointerWorldOffset;
		}

		#endregion

		#region IEndDragHandler implementation

		public void OnEndDrag(PointerEventData eventData)
		{
			if (eventData.button != 0)
				return;
			if (draggingInstance == null)
				return;

			bool isWorldPos = (eventData.hovered.Count == 0);
			if (!isWorldPos)
			{
				foreach (GameObject h in eventData.hovered)
				{
					if (object.ReferenceEquals(h, worldCanvas.gameObject))
					{
						isWorldPos = true;
						break;
					}
				}
			}
			if (!isWorldPos)
			{
				Destroy(draggingInstance.gameObject);
			}
			else
			{
				//This is _way_ too complicated, and totally misplaced here. Is there a better way?

				Vector2 newPos = worldCanvas.worldCamera.ScreenToWorldPoint(draggingInstance.rectTransform.position);
				draggingInstance.GetComponent<Image>().raycastTarget = true;
				draggingInstance.rectTransform.SetParent(worldCanvas.transform, false);
				draggingInstance.rectTransform.position = newPos;
				draggingInstance.canvas = worldCanvas;
				draggingInstance.canvasRectTransform = (RectTransform)worldCanvas.transform;
				draggingInstance.IsSidebarChip = false;

				draggingInstance.inPorts = new PortUi[inPorts.Length];
				for (int i = 0; i < inPorts.Length; i++)
				{
					draggingInstance.inPorts[i] = draggingInstance.transform.GetChild(inPorts[i].transform.GetSiblingIndex()).GetComponent<PortUi>();
					draggingInstance.inPorts[i].chipUi = draggingInstance;
					draggingInstance.inPorts[i].Port = draggingInstance.Chip.inputPorts[i];
				}
				draggingInstance.outPorts = new PortUi[outPorts.Length];
				for (int i = 0; i < outPorts.Length; i++)
				{
					draggingInstance.outPorts[i] = draggingInstance.transform.GetChild(outPorts[i].transform.GetSiblingIndex()).GetComponent<PortUi>();
					draggingInstance.outPorts[i].chipUi = draggingInstance;
					draggingInstance.outPorts[i].Port = draggingInstance.Chip.outputPorts[i];
				}
			}
			draggingInstance = null;
		}

		#endregion
	}

}
