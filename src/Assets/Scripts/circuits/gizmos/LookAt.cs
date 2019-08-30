namespace AssemblyCSharp.gizmos
{
	public class LookAt : Gizmo
	{
		private bool isActive = false;

		public LookAt(CircuitManager manager) : base(manager, 1)
		{
		}

		protected override void EvaluateOutputs()
		{
			isActive = InBool(0);
		}

		override protected NodeSetting[] CreateSettings()
		{
			return new NodeSetting[]
			{
				NodeSetting.CreateSetting(NodeSetting.SettingType.TagToFollow)
			};
		}

		public override string GetGizmoValueString()
		{
			return isActive ? "#" + settings[0].currentValue : "";
		}

		public override void Reset() { }
	}
}
