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


            int i = 0;
            foreach (PortUi portUi in portsUis)
            {
                portUi.nodeUi = this;
                portUi.Port = Node.inputPorts[i];
                portUi.PortIndex = i;

                inPorts[i] = portUi;

                i++;
            }
        }

        protected override CircuitNode CreateNode(CircuitManager manager)
        {
            sign = new Sign(manager);
            sign.TextChanged += Sign_TextChanged;
            return sign;
        }

        private void Sign_TextChanged(string message, bool limitLength)
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
            sign.TextChanged -= Sign_TextChanged;
            base.OnDestroy();
        }
    }
}