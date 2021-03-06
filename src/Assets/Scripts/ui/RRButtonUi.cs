﻿using TMPro;

namespace AssemblyCSharp
{
	using UnityEngine;
	using UnityEngine.UI;

	public class RRButtonUi : NodeUi
	{
		internal RRButton button;
		private RRButtonCenterUi centerUi;
		public TextMeshProUGUI buttonText;

		void Start()
		{
			DebugUtils.Assert(inPorts == null);
			DebugUtils.Assert(outPorts == null);
			inPorts = new PortUi[0];
			outPorts = new PortUi[3];
			statePorts = new PortUi[0];

			int portIndex = 0;
			foreach (Transform child in transform)
			{
				if (centerUi == null)
					centerUi = child.GetComponent<RRButtonCenterUi>();
				PortUi port = child.GetComponent<PortUi>();
				if (port == null)
					continue;
				port.nodeUi = this;
				port.Port = button.outputPorts[portIndex];
				port.PortIndex = portIndex;
				outPorts[portIndex] = port;
				portIndex++;
			}
			DebugUtils.Assert(portIndex == 3);

			button.ButtonTextChanged += Button_ButtonTextChanged;
			Button_ButtonTextChanged(button, button.ButtonText);
		}

		private void Button_ButtonTextChanged(RRButton source, string text)
		{
			buttonText.text = text;
		}

		protected override CircuitNode CreateNode(CircuitManager manager)
		{
			return button = new RRButton(manager);
		}

		protected override void OnMovedToWorld()
		{
			if (centerUi != null)
				centerUi.RaycastTarget = true;
		}
	}
}
