namespace AssemblyCSharp
{
	public abstract class Chip : CircuitNode
	{
		public Chip(CircuitManager manager, int inputCount, int outputCount, bool hasReset) : base(manager, inputCount, outputCount, hasReset)
		{
		}

		public abstract int IconIndex { get; }
	}
}