namespace AssemblyCSharp
{
	public class DataConnection : Connection
	{
		public OutputPort SourceDataPort { get; private set; }
		public InputPort TargetDataPort { get; private set; }

		public override Port SourcePort
		{
			get
			{
				return SourceDataPort;
			}
		}

		public override Port TargetPort
		{
			get
			{
				return TargetDataPort;
			}
		}

		public DataConnection(OutputPort source, InputPort target)
		{
			SourceDataPort = source;
			TargetDataPort = target;
		}
	}
}
