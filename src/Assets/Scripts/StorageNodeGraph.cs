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
			public string uiType;
			public string typeParams;
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
				NodeUi ui = UiManager.GetUi(node);
				storageNode.uiType = ui.GetType().FullName;
				storageNode.typeParams = ui.GetParams();
				storageNode.position = ui.transform.position;
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

		public void Restore(RRCSManager manager)
		{
			Dictionary<string, GameObject> typeMap = new Dictionary<string, GameObject>();
			typeMap.Add(typeof(ChipUi).FullName, manager.chipUiPrefab);
			typeMap.Add(typeof(RRButtonUi).FullName, manager.rRButtonPrefab);
			typeMap.Add(typeof(StageLightUi).FullName, manager.stageLightPrefab);

			foreach (StorageNode storageNode in graph)
			{
				if (!typeMap.ContainsKey(storageNode.uiType))
					throw new InvalidOperationException("Invalid node type " + storageNode.uiType + ".");
				GameObject go = GameObject.Instantiate(typeMap[storageNode.uiType], manager.WorldCanvas.transform);
				NodeUi ui = go.GetComponent<NodeUi>();
				ui.ParseParams(storageNode.typeParams);
				go.transform.position = storageNode.position;
				CircuitNode node = ui.Node;

				if (storageNode.settings.Length != node.settings.Length)
					throw new InvalidOperationException("Number of settings does not match for type " + storageNode.uiType + ".");
				for (int j = 0; j < storageNode.settings.Length; j++)
				{
					NodeSetting setting = node.settings[j];
					StorageNodeGrahp.NodeSettingContainer settingContainer = storageNode.settings[j];
					if (settingContainer.type != setting.type)
						throw new InvalidOperationException("Setting at " + j + " does not match for type " + storageNode.uiType + ".");
					setting.ParseValue(settingContainer.value);
				}
			}
			Debug.Assert(manager.circuitManager.Nodes.Count == graph.Length);

			List<CircuitNode> nodes = manager.circuitManager.Nodes;
			for (int nodeIndex = 0; nodeIndex < graph.Length; nodeIndex++)
			{
				CircuitNode node = nodes[nodeIndex];
				StorageNode storageNode = graph[nodeIndex];
				for (int portIndex = 0; portIndex < storageNode.connections.Length; portIndex++)
				{
					NodeConnection connection = storageNode.connections[portIndex];
					if (connection.nodeIndex >= 0 && connection.portIndex >= 0)
					{
						InputPort port = node.inputPorts[portIndex];
						OutputPort connectedPort = nodes[connection.nodeIndex].outputPorts[connection.portIndex];
						port.Connect(connectedPort);
						//connectedPort.Connect(port);
					}
				}
			}
		}
	}
}