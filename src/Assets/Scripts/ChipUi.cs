namespace AssemblyCSharp
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.EventSystems;

	public class ChipUi : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
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

		public PortUi in0;
		public PortUi inR;
		public PortUi out0;
		public PortUi outR;
		public Image icon;
		public Sprite sprite;

		public int inPortCount = 1;
		public int outPortCount = 1;
		public bool hasReset = true;
		[HideInInspector]
		public bool skipSetup = false;

		private PortUi[] inPorts;
		private PortUi[] outPorts;

		private Vector3 pointerWorldOffset;
		private RectTransform rectTransform;
		private Canvas canvas;
		private static Canvas worldCanvas;
		private RectTransform canvasRectTransform;
		private ChipUi draggingInstance;

		public bool IsSidebarChip{ get; private set; }

		void Awake ()
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
			}
			rectTransform = (RectTransform)transform;
			canvas = GetComponentInParent<Canvas>();
			canvasRectTransform = (RectTransform)canvas.transform;
			IsSidebarChip = (canvas.tag == "Sidebar");

			if (!skipSetup)
			{
				skipSetup = true;
				int maxTotalPortCount = Mathf.Max(inPortCount, outPortCount) + (hasReset ? 1 : 0);
				bool useCompactFormat = false;
				if (maxTotalPortCount >= 4)
				{
					useCompactFormat = true;
					rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, ((3 * maxTotalPortCount - 1) * in0.RectTransform.sizeDelta.y) / 2);
				}
				SetupPorts(inPortCount, useCompactFormat, ref in0, ref inR, ref inPorts);
				SetupPorts(outPortCount, useCompactFormat, ref out0, ref outR, ref outPorts);
				if (sprite != null)
					icon.sprite = sprite;
			}
		}

		private void SetupPorts(int portCount, bool useCompactFormat, ref PortUi port0, ref PortUi resetPort, ref PortUi[] ports)
		{
			int totalPortCount = portCount + (hasReset ? 1 : 0);
			ports = new PortUi[totalPortCount];

			if (portCount <= 0)
				Destroy(port0);
			else
			{
				ports[0] = port0;
				port0.chipUi = this;
				RectTransform tr0 = port0.RectTransform;
				
				if (useCompactFormat)
				{
					tr0.anchoredPosition = new Vector2(tr0.anchoredPosition.x, -tr0.sizeDelta.y / 2);
					((RectTransform)resetPort.transform).anchoredPosition = new Vector2(0, tr0.sizeDelta.y / 2);
				}
				
				Vector2 offset = new Vector2(0, tr0.sizeDelta.y * -3 / 2);
				for (int i = 1; i < portCount; i++)
				{
					PortUi port = Instantiate<PortUi>(port0, transform);
					ports[i] = port;
					port.chipUi = this;
					port.RectTransform.anchoredPosition = tr0.anchoredPosition + i * offset;
					port.Image.color = portColors[i % portColors.Length];
				}
			}
			if (hasReset)
			{
				ports[portCount] = resetPort;
				resetPort.chipUi = this;
			}
			else
				Destroy(resetPort);
			port0 = null;
			resetPort = null;
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
				Vector2 newPos = worldCanvas.worldCamera.ScreenToWorldPoint(draggingInstance.rectTransform.position);
				draggingInstance.GetComponent<Image>().raycastTarget = true;
				draggingInstance.rectTransform.SetParent(worldCanvas.transform, false);
				draggingInstance.rectTransform.position = newPos;
				draggingInstance.canvas = worldCanvas;
				draggingInstance.canvasRectTransform = (RectTransform)worldCanvas.transform;
				draggingInstance.IsSidebarChip = false;
			}
			draggingInstance = null;
		}

		#endregion
	}

}
