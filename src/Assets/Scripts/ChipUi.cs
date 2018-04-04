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

		public GameObject in0;
		public GameObject inR;
		public GameObject out0;
		public GameObject outR;
		public Image icon;
		public Sprite sprite;

		public int inPortCount = 1;
		public int outPortCount = 1;
		public bool hasReset = true;
		[HideInInspector]
		public bool skipSetup = false;

		private GameObject[] inPorts;
		private GameObject[] outPorts;

		private Vector3 pointerWorldOffset;
		private RectTransform rectTransform;
		private Canvas canvas;
		private RectTransform canvasRectTransform;
		private ChipUi draggingInstance;

		void Awake ()
		{
			rectTransform = (RectTransform)transform;
			canvas = GetComponentInParent<Canvas>();
			canvasRectTransform = (RectTransform)canvas.transform;

			if (!skipSetup)
			{
				skipSetup = true;
				int maxTotalPortCount = Mathf.Max(inPortCount, outPortCount) + (hasReset ? 1 : 0);
				bool useCompactFormat = false;
				if (maxTotalPortCount >= 4)
				{
					useCompactFormat = true;
					RectTransform tr0 = (RectTransform)in0.transform;
					RectTransform rectTr = (RectTransform)transform;
					rectTr.sizeDelta = new Vector2(rectTr.sizeDelta.x, ((3 * maxTotalPortCount - 1) * tr0.sizeDelta.y) / 2);
				}
				setupPorts(inPortCount, useCompactFormat, ref in0, ref inR, ref inPorts);
				setupPorts(outPortCount, useCompactFormat, ref out0, ref outR, ref outPorts);
				if (sprite != null)
					icon.sprite = sprite;
			}
		}

		private void setupPorts(int portCount, bool useCompactFormat, ref GameObject port0, ref GameObject resetPort, ref GameObject[] ports)
		{
			int totalPortCount = portCount + (hasReset ? 1 : 0);
			ports = new GameObject[totalPortCount];

			if (portCount <= 0)
				Destroy(port0);
			else
			{
				ports[0] = port0;
				RectTransform tr0 = (RectTransform)port0.transform;
				
				if (useCompactFormat)
				{
					tr0.anchoredPosition = new Vector2(tr0.anchoredPosition.x, 0);
					((RectTransform)resetPort.transform).anchoredPosition = new Vector2();
				}
				
				Vector2 offset = new Vector2(0, tr0.sizeDelta.y * -3 / 2);
				for (int i = 1; i < portCount; i++)
				{
					GameObject port = Instantiate<GameObject>(port0, transform);
					var tr = port.transform.transform as RectTransform;
					tr.anchoredPosition = tr0.anchoredPosition + i * offset;

					Image image = port.GetComponent<Image>();
					image.color = portColors[i % portColors.Length];
				}
			}
			if (hasReset)
				ports[portCount] = resetPort;
			else
				Destroy(resetPort);
			port0 = null;
			resetPort = null;
		}

		#region IPointerDownHandler implementation

		public void OnPointerDown(PointerEventData eventData)
		{
			Vector2 n_originalLocalPointerPosition;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out n_originalLocalPointerPosition);
			pointerWorldOffset = rectTransform.InverseTransformPoint(n_originalLocalPointerPosition) - rectTransform.InverseTransformPoint(new Vector3());
		}

		#endregion

		#region IBeginDragHandler implementation

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (draggingInstance != null)
				return;
			if (canvas.tag != "Sidebar")
				return;
			draggingInstance = Instantiate(this, canvasRectTransform);
		}

		#endregion

		#region IDragHandler implementation

		public void OnDrag(PointerEventData eventData)
		{
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
			if (draggingInstance != null)
			{
				Destroy(draggingInstance.gameObject);
				draggingInstance = null;
			}
		}

		#endregion
	}

}
