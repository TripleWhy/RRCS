namespace AssemblyCSharp
{
	using System;

	public abstract class Gizmo : CircuitNode
	{
		public Gizmo(CircuitManager manager, int inputCount) :
			base(manager, inputCount, 0, false)
		{
		}

		public abstract string GetGizmoValueString();

		public abstract void ResetGizmo();
	}
}
