namespace AssemblyCSharp
{
    using System;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class SaveDataContainer
    {
        private Dictionary<CircuitNode, Transform> circuits;

        public SaveDataContainer()
        {
            circuits = new Dictionary<CircuitNode, Transform>();
            List<CircuitNode> nodes = RRCSManager.Instance.circuitManager.GetNodes();
            List<Transform> transforms = new List<Transform>();
      
            foreach(Transform child in RRCSManager.Instance.WorldCanvas.transform)
            {
                transforms.Add(child);
            }
   
            for(int i = 0; i < nodes.Count; i++)
            {
                circuits.Add(nodes[i], transforms[i]);
            }
        }
    }
}

