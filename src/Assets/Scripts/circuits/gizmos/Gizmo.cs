namespace AssemblyCSharp
{
	using System;

	public abstract class Gizmo : CircuitNode
	{
		public Gizmo(CircuitManager manager, int inputCount) :
			base(manager, inputCount, 0, false)
		{
		}

		protected override IConvertible DefaultInputValue(int inputIndex)
		{
			switch (inputIndex)
			{
				case 0:
					return false;
				case 1:
				case 2:
					return 0F;
				default:
					return base.DefaultInputValue(inputIndex);
			}
		}

		public abstract string GetGizmoValueString();

		public abstract void ResetGizmo();
	}
}
