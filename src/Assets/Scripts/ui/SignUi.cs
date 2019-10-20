using TMPro;
using UnityEngine;

namespace AssemblyCSharp
{
	public class SignUi : NodeUi
	{
		public TextMeshProUGUI text;
		private Sign sign;
		public PortUi[] portsUis;

		void Start()
		{
			inPorts = new PortUi[4];
			outPorts = new PortUi[0];
			statePorts = new PortUi[0];


			for (int i = 0; i < portsUis.Length; ++i)
			{
				PortUi portUi = portsUis[i];
				portUi.nodeUi = this;
				portUi.Port = Node.inputPorts[i];
				portUi.PortIndex = i;
				inPorts[i] = portUi;
			}

			sign.DisplayTextChanged += Sign_DisplayTextChanged;
			Sign_DisplayTextChanged(sign.DisplayText, sign.DisplayTextIsLimited);
		}

		protected override CircuitNode CreateNode(CircuitManager manager)
		{
			return sign = new Sign(manager);
		}

		private void Sign_DisplayTextChanged(string message, bool limitLength)
		{
			text.text = message;
			text.enableWordWrapping = !limitLength;
		}

		protected override void OnMovedToWorld()
		{
			transform.localScale = new Vector3(1, 1, 1);
		}

		protected new void OnDestroy()
		{
			sign.DisplayTextChanged -= Sign_DisplayTextChanged;
			base.OnDestroy();
		}
	}
}
