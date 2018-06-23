namespace AssemblyCSharp
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	[Serializable]
	public class StorageNodeGrahp
	{
		[Serializable]
		public class StorageNode
		{
			public string nodeType;
			public Vector2 position;
			public NodeSettingContainer[] settings;
			public NodeConnection[] connections;
		}

		[Serializable]
		public class NodeSettingContainer
		{
			public NodeSetting.SettingType type;
			public string value;
		}

		[Serializable]
		public class NodeConnection
		{
			public int nodeIndex = -1;
			public int portIndex = -1;
		}

		public StorageNode[] graph;

		public void Fill()
		{
			List<CircuitNode> nodes = RRCSManager.Instance.circuitManager.Nodes;
			graph = new StorageNode[nodes.Count];
			for (int i = 0; i < graph.Length; i++)
			{
				CircuitNode node = nodes[i];
				Debug.Assert(node.RingEvaluationPriority == i);

				StorageNode storageNode = new StorageNode();
				storageNode.nodeType = node.GetType().FullName;
				storageNode.position = UiManager.GetUi(node).transform.position;
				storageNode.settings = new NodeSettingContainer[node.settings.Length];
				for (int j = 0; j < storageNode.settings.Length; j++)
				{
					NodeSetting setting = node.settings[j];
					NodeSettingContainer settingContainer = new NodeSettingContainer();
					settingContainer.type = setting.type;
					settingContainer.value = setting.currentValue.ToString();
					storageNode.settings[j] = settingContainer;
				}

				graph[i] = storageNode;
			}

			for (int nodeIndex = 0; nodeIndex < graph.Length; nodeIndex++)
			{
				CircuitNode node = nodes[nodeIndex];
				StorageNode storageNode = graph[nodeIndex];
				storageNode.connections = new NodeConnection[node.inputPorts.Length];
				for (int portIndex = 0; portIndex < node.inputPorts.Length; portIndex++)
				{
					InputPort port = node.inputPorts[portIndex];
					if (port.IsConnected)
					{
						NodeConnection connection = new NodeConnection();
						OutputPort connectedPort = (OutputPort)port.connectedPorts[0];
						connection.nodeIndex = connectedPort.node.RingEvaluationPriority;
						connection.portIndex = Array.IndexOf(connectedPort.node.outputPorts, connectedPort);
						storageNode.connections[portIndex] = connection;
					}
				}
			}
		}
	}
}