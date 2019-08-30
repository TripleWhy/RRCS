namespace AssemblyCSharp.gizmos
{
	public class Clamp : Gizmo
	{
		private bool isClamped = true;

		public Clamp(CircuitManager manager)
			: base(manager, 1)
		{
		}

		protected override void EvaluateOutputs()
		{
			isClamped = !InBool(0);
		}

		public override string GetGizmoValueString()
		{
			return isClamped ? "Clamped" : "Released";
		}

		public override void Reset() { }
	}
}
