namespace AssemblyCSharp
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	public class ChipUi : MonoBehaviour
	{
		private static Color[] portColors =
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

		private GameObject[] inPorts;
		private GameObject[] outPorts;

		void Awake ()
		{
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
	}

}