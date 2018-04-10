namespace AssemblyCSharp
{
	public abstract class Chip : CircuitNode
	{
		public Chip(CircuitManager manager, int inputCount, int outputCount, bool hasReset) : base(manager, inputCount, outputCount, hasReset)
		{
		}

		public static int ToInt(bool b)
		{
			return b ? 1 : 0;
		}

		public static bool ToBool(int i)
		{
			return i != 0;
		}

		public static int ToInt(Port p)
		{
			return p.GetValue();
		}

		public static bool ToBool(Port p)
		{
			return ToBool(ToInt(p));
		}

		public abstract int IconIndex { get; }
	}
}