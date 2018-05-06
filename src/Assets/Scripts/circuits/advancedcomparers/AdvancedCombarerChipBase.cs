namespace AssemblyCSharp
{
	public abstract class AdvancedCombarerChipBase : Chip
	{
		protected AdvancedCombarerChipBase(CircuitManager manager) : base(manager, 4, 2, true)
		{
		}

		override protected void EvaluateOutputs()
		{
			if (Compare(inputPorts[0].GetValue(), inputPorts[1].GetValue()))
			{
				outputPorts[0].Value = inputPorts[2].GetValue();
				outputPorts[1].Value = 0;
			}
			else
			{
				outputPorts[0].Value = 0;
				outputPorts[1].Value = inputPorts[3].GetValue();
			}
		}

		abstract protected bool Compare(int a, int b);
	}
}

