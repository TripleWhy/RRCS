namespace AssemblyCSharp
{
	public abstract class SimpleCombarerChipBase : Chip
	{
		protected SimpleCombarerChipBase(CircuitManager manager) : base(manager, 2, 2, true)
		{
		}

		override protected void EvaluateOutputs()
		{
			outputPorts[0].Value = ToInt(Compare(inputPorts[0].GetValue(), inputPorts[1].GetValue()));
			outputPorts[1].Value = 1 - outputPorts[0].Value;
		}

		abstract protected bool Compare(int a, int b);
	}
}

