﻿namespace AssemblyCSharp
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	public class ChipUi : NodeUi
	{
		public enum ChipType
		{
			None,
			Add, Subtract, Multiply, Divide, Modulo,
			Equal, NotEqual, GreaterThan, GreaterThanOrEqual, LessThan, LessThanOrEqual,
			AdvancedEqual, AdvancedNotEqual, AdvancedGreaterThan, AdvancedGreaterThanOrEqual, AdvancedLessThan, AdvancedLessThanOrEqual,
			And, Or, Not,
			Variable,
			Output, Message,
			Delay, Timer,
			StartGame, GameState, SetScore, Score,
			SFX, Random, Selector,
			Respawn, PlayerHit,
			StateMachine, State,

			ExtBreakpoint,
			ExtBitwiseComplement, ExtBitshiftLeft, ExtBitshiftRight, ExtBitwiseAnd, ExtBitwiseOr, ExtBitwiseXor,
			ExtValueStore, ExtBuffer, ExtArray, ExtMultiplexer,
			ExtMathConstants, ExtMathAbs, ExtMathSign, ExtMathClamp, ExtMathRound, ExtMathCeil, ExtMathFloor,
			ExtMathSin, ExtMathCos, ExtMathTan, ExtMathAtan, ExtMathAtan2,
			ExtMathSqrt, ExtMathExp, ExtMathPow, ExtMathLog,
		};

		public ChipType type;
		public PortUi in0;
		public PortUi inR;
		public PortUi out0;
		public PortUi outR;
		public PortUi state;
		public Image icon;
		private int lastPriority;

		[HideInInspector]
		[SerializeField]
		private bool skipSetup = false;

		private static Sprite[] icons;

		new void Awake()
		{
			base.Awake();
			if (icons == null)
				icons = Resources.LoadAll<Sprite>("iconsWhite");
		}

		void Start()
		{
			if (!skipSetup)
			{
				skipSetup = true;
				int maxTotalPortCount = Mathf.Max(TotalInPortCount, TotalOutPortCount);
				bool useCompactFormat = false;
				if (maxTotalPortCount >= 4)
				{
					useCompactFormat = true;
					rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x,
						((3 * maxTotalPortCount - 1) * in0.RectTransform.sizeDelta.y) / 2);
				}

				SetupPorts(InPortCount, Chip.inputPorts, useCompactFormat, ref in0, ref inR, ref inPorts);
				SetupPorts(OutPortCount, Chip.outputPorts, useCompactFormat, ref out0, ref outR, ref outPorts);
				PortUi nullPort = null;
				SetupPorts(Chip.statePort != null ? 1 : 0, new Port[] {Chip.statePort}, false, ref state,
					ref nullPort, ref statePorts);
				Sprite sprite = GetSprite();
				if (sprite != null)
					icon.sprite = sprite;
			}
		}

		void Update()
		{
			if (evaluationIndexText != null && evaluationIndexText.gameObject.activeInHierarchy)
			{
				int priority = Chip.RingEvaluationPriority;
				if (priority != lastPriority)
				{
					lastPriority = priority;
					evaluationIndexText.text = priority.ToString();
				}
			}
		}

		public Chip Chip
		{
			get
			{
				return (Chip) Node;
			}
			set
			{
				Node = value;
			}
		}

		private void SetupPorts(int portCount, Port[] ports, bool useCompactFormat, ref PortUi port0UI,
			ref PortUi resetPortUi, ref PortUi[] portUIs)
		{
			DebugUtils.Assert(ports != null);
			DebugUtils.Assert(portUIs == null);
			portUIs = new PortUi[ports.Length];

			if (portCount <= 0)
				Destroy(port0UI.gameObject);
			else
			{
				portUIs[0] = port0UI;
				port0UI.nodeUi = this;
				port0UI.Port = ports[0];
				RectTransform tr0 = port0UI.RectTransform;

				if (useCompactFormat)
				{
					tr0.anchoredPosition = new Vector2(tr0.anchoredPosition.x, -tr0.sizeDelta.y / 2);
					if (resetPortUi != null)
					{
						((RectTransform) resetPortUi.transform).anchoredPosition = new Vector2(0, tr0.sizeDelta.y / 2);
					}
				}

				Vector2 offset = new Vector2(0, tr0.sizeDelta.y * -3 / 2);
				for (int i = 1; i < portCount; i++)
				{
					PortUi portUi = Instantiate<PortUi>(port0UI, transform);
					portUIs[i] = portUi;
					portUi.nodeUi = this;
					portUi.Port = ports[i];
					portUi.RectTransform.anchoredPosition = tr0.anchoredPosition + i * offset;
					portUi.PortIndex = i;
				}
			}

			if (resetPortUi != null)
			{
				if (HasReset)
				{
					portUIs[portCount] = resetPortUi;
					resetPortUi.nodeUi = this;
					resetPortUi.Port = ports[portCount];
				}
				else
				{
					Destroy(resetPortUi.gameObject);
				}
			}

			port0UI = null;
			resetPortUi = null;
		}

		protected override CircuitNode CreateNode(CircuitManager manager)
		{
			switch (type)
			{
				case ChipType.Add:
					return new AddChip(manager);
				case ChipType.Subtract:
					return new SubtractChip(manager);
				case ChipType.Multiply:
					return new MultiplyChip(manager);
				case ChipType.Divide:
					return new DivideChip(manager);
				case ChipType.Modulo:
					return new ModuloChip(manager);
				case ChipType.And:
					return new AndChip(manager);
				case ChipType.Or:
					return new OrChip(manager);
				case ChipType.Not:
					return new NotChip(manager);
				case ChipType.Equal:
					return new SimpleEqualsChip(manager);
				case ChipType.NotEqual:
					return new SimpleNotEqualsChip(manager);
				case ChipType.GreaterThan:
					return new SimpleGreaterThanChip(manager);
				case ChipType.GreaterThanOrEqual:
					return new SimpleGreaterThanOrEqualChip(manager);
				case ChipType.LessThan:
					return new SimpleLessThanChip(manager);
				case ChipType.LessThanOrEqual:
					return new SimpleLessThanOrEqualChip(manager);
				case ChipType.AdvancedEqual:
					return new AdvancedEqualsChip(manager);
				case ChipType.AdvancedNotEqual:
					return new AdvancedNotEqualsChip(manager);
				case ChipType.AdvancedGreaterThan:
					return new AdvancedGreaterThanChip(manager);
				case ChipType.AdvancedGreaterThanOrEqual:
					return new AdvancedGreaterThanOrEqualChip(manager);
				case ChipType.AdvancedLessThan:
					return new AdvancedLessThanChip(manager);
				case ChipType.AdvancedLessThanOrEqual:
					return new AdvancedLessThanOrEqualChip(manager);
				case ChipType.Variable:
					return new VariableChip(manager);
				case ChipType.Delay:
					return new DelayChip(manager);
				case ChipType.Timer:
					return new TimerChip(manager);
				case ChipType.Random:
					return new RandomChip(manager);
				case ChipType.Selector:
					return new SelectorChip(manager);
				case ChipType.StateMachine:
					return new StateMachineChip(manager);
				case ChipType.State:
					return new StateChip(manager);
				case ChipType.ExtBreakpoint:
					return new BreakpointChip(manager);
				case ChipType.ExtBitwiseComplement:
					return new BitwiseComplementChip(manager);
				case ChipType.ExtBitshiftLeft:
					return new BitshiftLeftChip(manager);
				case ChipType.ExtBitshiftRight:
					return new BitshiftRightChip(manager);
				case ChipType.ExtBitwiseAnd:
					return new BitwiseAndChip(manager);
				case ChipType.ExtBitwiseOr:
					return new BitwiseOrChip(manager);
				case ChipType.ExtBitwiseXor:
					return new BitwiseXorChip(manager);
				case ChipType.ExtValueStore:
					return new ValueStoreChip(manager);
				case ChipType.ExtBuffer:
					return new BufferChip(manager);
				case ChipType.ExtArray:
					return new ArrayChip(manager);
				case ChipType.ExtMultiplexer:
					return new MultiplexerChip(manager);
				case ChipType.ExtMathConstants:
					return new MathConstantsChip(manager);
				case ChipType.ExtMathAbs:
					return new AbsChip(manager);
				case ChipType.ExtMathSign:
					return new SignChip(manager);
				case ChipType.ExtMathClamp:
					return new ClampChip(manager);
				case ChipType.ExtMathRound:
					return new RoundChip(manager);
				case ChipType.ExtMathCeil:
					return new CeilChip(manager);
				case ChipType.ExtMathFloor:
					return new FloorChip(manager);
				case ChipType.ExtMathSin:
					return new SinChip(manager);
				case ChipType.ExtMathCos:
					return new CosChip(manager);
				case ChipType.ExtMathTan:
					return new TanChip(manager);
				case ChipType.ExtMathAtan:
					return new AtanChip(manager);
				case ChipType.ExtMathAtan2:
					return new Atan2Chip(manager);
				case ChipType.ExtMathSqrt:
					return new SqrtChip(manager);
				case ChipType.ExtMathExp:
					return new ExpChip(manager);
				case ChipType.ExtMathPow:
					return new PowChip(manager);
				case ChipType.ExtMathLog:
					return new LogChip(manager);
				default:
					return null;
			}
		}

		private Sprite GetSprite()
		{
			if (Chip.IconIndex == -1)
				return null;
			return icons[Chip.IconIndex];
		}

		protected override void OnMovedToWorld()
		{
			if (!skipSetup)
				return;
			DebugUtils.Assert(inPorts == null);
			DebugUtils.Assert(outPorts == null);
			inPorts = new PortUi[TotalInPortCount];
			outPorts = new PortUi[TotalOutPortCount];
			statePorts = new PortUi[TotalStatePortCount];
			int inPortIndex = 0;
			int outPortIndex = 0;
			int statePortIndex = 0;
			foreach (Transform child in transform)
			{
				PortUi port = child.GetComponent<PortUi>();
				if (port == null)
					continue;
				port.nodeUi = this;
				if (port.isInput)
				{
					int index = port.IsReset ? InPortCount : (inPortIndex++);
					DebugUtils.Assert(inPorts[index] == null);
					inPorts[index] = port;
					port.Port = Node.inputPorts[index];
				}
				else if (port.isState)
				{
					int index = statePortIndex++;
					DebugUtils.Assert(statePorts[index] == null);
					statePorts[index] = port;
					port.Port = Node.statePort;
				}
				else
				{
					int index = port.IsReset ? OutPortCount : (outPortIndex++);
					DebugUtils.Assert(outPorts[index] == null);
					outPorts[index] = port;
					port.Port = Node.outputPorts[index];
				}
			}
			DebugUtils.Assert(!Array.Exists(inPorts, element => element == null));
			DebugUtils.Assert(!Array.Exists(outPorts, element => element == null));
		}

		public override string GetParams()
		{
			return type.ToString();
		}

		public override void ParseParams(string parameters)
		{
			DebugUtils.Assert(type == (ChipType) Enum.Parse(typeof(ChipType), parameters));
		}
	}
}