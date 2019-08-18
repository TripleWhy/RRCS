namespace AssemblyCSharp
{
	using UnityEngine;
	using UnityEngine.UI;

	public class StageLightUi : NodeUi
	{
		private StageLight stageLight;
		private Image lightCone;
		private Image lamp;

		new void Awake()
		{
			base.Awake();
			foreach (Transform child in transform)
			{
				Image img = child.GetComponent<Image>();
				if (img != null)
				{
					if (lightCone == null)
					{
						lightCone = img;
					}
					else if (lamp == null)
					{
						lamp = img;
						break;
					}
				}
			}
		}

		void Start()
		{
			DebugUtils.Assert(inPorts == null);
			DebugUtils.Assert(outPorts == null);
			inPorts = new PortUi[3];
			outPorts = new PortUi[0];
			statePorts = new PortUi[0];

			int portIndex = 0;
			foreach (Transform child in transform)
			{
				Image img = child.GetComponent<Image>();
				if (img != null)
				{
					if (lightCone == null)
					{
						lightCone = img;
						continue;
					}
					else if (lamp == null)
					{
						lamp = img;
						continue;
					}
				}
				PortUi port = child.GetComponent<PortUi>();
				if (port == null)
					continue;
				port.nodeUi = this;
				port.Port = stageLight.inputPorts[portIndex];
				port.PortIndex = portIndex;
				inPorts[portIndex] = port;
				portIndex++;
			}
			DebugUtils.Assert(lightCone != null);
			DebugUtils.Assert(lamp != null);
			DebugUtils.Assert(portIndex == 3);

			stageLight.ColorChanged += StageLight_ColorChanged;
			StageLight_ColorChanged(stageLight, stageLight.color);
		}

		protected override CircuitNode CreateNode(CircuitManager manager)
		{
			return stageLight = new StageLight(manager);
		}

		private void StageLight_ColorChanged(StageLight source, StageLight.Color color)
		{
			if (color.alpha == 0)
				lightCone.gameObject.SetActive(false);
			else
			{
				lightCone.gameObject.SetActive(true);
				lightCone.color = new Color32(color.red, color.green, color.blue, color.alpha);
			}
		}

		protected override void EnableRaycast(bool on)
		{
			lamp.raycastTarget = on;
		}
	}
}
