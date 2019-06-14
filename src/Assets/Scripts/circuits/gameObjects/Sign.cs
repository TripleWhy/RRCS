using System;
using TMPro;
using UnityEngine;

namespace AssemblyCSharp
{
    public class Sign : CircuitNode
    {
        public delegate void TextChangedEventHandler(string message);

        public event TextChangedEventHandler TextChanged = delegate { };

        public Sign(CircuitManager manager) : base(manager, 4, 0, false)
        {
        }

        protected override NodeSetting[] CreateSettings()
        {
            return new NodeSetting[]
            {
                NodeSetting.CreateSetting(NodeSetting.SettingType.Message0),
                NodeSetting.CreateSetting(NodeSetting.SettingType.Message1),
                NodeSetting.CreateSetting(NodeSetting.SettingType.Message2),
                NodeSetting.CreateSetting(NodeSetting.SettingType.Message3),
                NodeSetting.CreateSetting(NodeSetting.SettingType.Message4),
                NodeSetting.CreateSetting(NodeSetting.SettingType.LimitMessage),
            };
        }

        protected override void EvaluateOutputs()
        {
            string message = (string) settings[Math.Max(0, Math.Min(4, inputPorts[3].GetValue()))].currentValue;

            bool limit = (bool) settings[5].currentValue;
            
            if (limit)
                message = message.Substring(0, Math.Min(message.Length, 20));

            message = message.Replace("{R}", inputPorts[0].GetValue().ToString());
            message = message.Replace("{G}", inputPorts[1].GetValue().ToString());
            message = message.Replace("{B}", inputPorts[2].GetValue().ToString());

            if (limit)
                message = message.Substring(0, Math.Min(message.Length, 20));

            TextChanged(message);
        }
    }
}