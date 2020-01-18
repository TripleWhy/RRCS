namespace AssemblyCSharp
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
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
			public NodeTransition[] transitions;
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

		[Serializable]
		public class NodeTransition
		{
			public int sourceNodeIndex = -1;
			public int targetNodeIndex = -1;
			public NodeConnection transitionEnabledConnection;
		}

		public StorageNode[] graph;

		public void Fill()
		{
			List<CircuitNode> nodes = RRCSManager.Instance.circuitManager.Nodes;
			graph = new StorageNode[nodes.Count];
			for (int i = 0; i < graph.Length; i++)
			{
				CircuitNode node = nodes[i];
				DebugUtils.Assert(node.RingEvaluationPriority == i);

				StorageNode storageNode = new StorageNode();
				NodeUi ui = UiManager.GetUi(node);
				storageNode.uiType = ui.GetType().FullName;
				storageNode.typeParams = ui.GetParams();
				storageNode.position = ui.transform.position;
				storageNode.settings = new NodeSettingContainer[node.settings.Length];
				for (int j = 0; j < storageNode.settings.Length; j++)
				{
					NodeSetting setting = node.settings[j];
					storageNode.settings[j] = new NodeSettingContainer
					{
						type = setting.type,
						value = setting.currentValue.ToString()
					};
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
						OutputPort connectedPort = ((DataConnection)port.connections[0]).SourceDataPort;
						connection.nodeIndex = connectedPort.RingEvaluationPriority;
						connection.portIndex = Array.IndexOf(connectedPort.Node.outputPorts, connectedPort);
						storageNode.connections[portIndex] = connection;
					}
				}
			}

			for (int nodeIndex = 0; nodeIndex < graph.Length; nodeIndex++)
			{
				CircuitNode node = nodes[nodeIndex];
				StorageNode storageNode = graph[nodeIndex];

				if (node.statePort != null)
				{
					List<StateMachineTransition> outgoingTransitions = node.statePort.GetOutgoingTransitions().ToList();
					storageNode.transitions = new NodeTransition[outgoingTransitions.Count];

					for (int transitionIndex = 0; transitionIndex < storageNode.transitions.Length; transitionIndex++)
					{
						StateMachineTransition outgoingTransition = outgoingTransitions[transitionIndex];
						NodeTransition transition = new NodeTransition
						{
							sourceNodeIndex = outgoingTransition.SourceStatePort.RingEvaluationPriority,
							targetNodeIndex = outgoingTransition.TargetStatePort.RingEvaluationPriority
						};

						if (outgoingTransition.TransitionEnabledPort != null && outgoingTransition.TransitionEnabledPort.IsConnected)
						{
							OutputPort connectedPort = ((DataConnection)outgoingTransition.TransitionEnabledPort.connections[0]).SourceDataPort;
							NodeConnection connection = new NodeConnection
							{
								nodeIndex = connectedPort.RingEvaluationPriority,
								portIndex = Array.IndexOf(connectedPort.Node.outputPorts, connectedPort)
							};
							transition.transitionEnabledConnection = connection;
						}
						storageNode.transitions[transitionIndex] = transition;
					}
				}
				else
				{
					storageNode.transitions = new NodeTransition[0];
				}
			}
		}

		public IEnumerator Restore(RRCSManager manager)
		{
			Dictionary<string, GameObject> typeMap = new Dictionary<string, GameObject>();

			typeMap.Add(typeof(RRButtonUi).FullName,
				manager.NodeUiPrefabRoot.GetComponentInChildren<RRButtonUi>(true).gameObject);
			typeMap.Add(typeof(StageLightUi).FullName,
				manager.NodeUiPrefabRoot.GetComponentInChildren<StageLightUi>(true).gameObject);
			typeMap.Add(typeof(SignUi).FullName,
				manager.NodeUiPrefabRoot.GetComponentInChildren<SignUi>(true).gameObject);
			var chipPrefabs = manager.NodeUiPrefabRoot.GetComponentsInChildren<ChipUi>(true);
			foreach (var prefab in chipPrefabs)
			{
				typeMap.Add(Enum.GetName(typeof(ChipUi.ChipType), prefab.type), prefab.gameObject);
			}
			var gizmoPrefabs = manager.NodeUiPrefabRoot.GetComponentsInChildren<GizmoUi>(true);
			foreach (var prefab in gizmoPrefabs)
			{
				typeMap.Add(Enum.GetName(typeof(GizmoUi.GizmoType), prefab.type), prefab.gameObject);
			}

			Dictionary<NodeSetting.SettingType, int> settingsTypeIndex = new Dictionary<NodeSetting.SettingType, int>();
			foreach (StorageNode storageNode in graph)
			{
				settingsTypeIndex.Clear();
				var typeKey = storageNode.uiType;
				if (typeKey == typeof(ChipUi).FullName)
					typeKey = storageNode.typeParams;
				if (typeKey == typeof(GizmoUi).FullName)
					typeKey = storageNode.typeParams;

				if (!typeMap.ContainsKey(typeKey))
					throw new InvalidOperationException("Invalid node type " + typeKey + ".");
				GameObject go = GameObject.Instantiate(typeMap[typeKey], manager.WorldCanvas.transform);
				go.transform.SetAsFirstSibling();
				NodeUi ui = go.GetComponent<NodeUi>();
				ui.ParseParams(storageNode.typeParams);
				go.transform.position = storageNode.position;
				CircuitNode node = ui.Node;

				foreach (NodeSettingContainer settingContainer in storageNode.settings)
				{
					int offset;
					settingsTypeIndex.TryGetValue(settingContainer.type, out offset);
					int index = Array.FindIndex(node.settings, offset, s => s.type == settingContainer.type);
					if (index < 0)
					{
						Debug.Log("Stored setting " + settingContainer.type + " for type " + typeKey + " not found with offset " + offset + ".");
						continue;
					}
					NodeSetting setting = node.settings[index];
					DebugUtils.Assert(settingContainer.type == setting.type);
					setting.ParseValue(settingContainer.value);
					settingsTypeIndex[settingContainer.type] = index + 1;
				}
			}
			DebugUtils.Assert(manager.circuitManager.Nodes.Count == graph.Length);

			yield return 0;
			
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
					}
				}

				if (storageNode.transitions != null)
				{
					foreach (var transition in storageNode.transitions)
					{
						var sourceStatePort = nodes[transition.sourceNodeIndex].statePort;
						sourceStatePort.Connect(nodes[transition.targetNodeIndex].statePort);

						if (transition.transitionEnabledConnection != null &&
						    transition.transitionEnabledConnection.nodeIndex >= 0 &&
						    transition.transitionEnabledConnection.portIndex >= 0)
						{
							StateMachineTransition newTransition =
								(StateMachineTransition) sourceStatePort.connections[
									sourceStatePort.connections.Count - 1];

							var connectedNode = nodes[transition.transitionEnabledConnection.nodeIndex];
							var connectedPort =
								connectedNode.outputPorts[transition.transitionEnabledConnection.portIndex];
							newTransition.TransitionEnabledPort.Connect(connectedPort);
						}
					}
				}
			}
		}
	}
}