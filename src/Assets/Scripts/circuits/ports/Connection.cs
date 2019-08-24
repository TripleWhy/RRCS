namespace AssemblyCSharp
{
	public abstract class Connection
	{
		public abstract Port SourcePort { get; }
		public abstract Port TargetPort { get; }

		public virtual void Disconnect()
		{
			TargetPort.Disconnect(this);
			SourcePort.Disconnect(this);
		}
	}
}