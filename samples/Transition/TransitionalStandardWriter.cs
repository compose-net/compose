namespace Transition
{
	internal class TransitionalStandardWriter : Compose.DirectTransition<IWriter, StandardWriter>, IWriter
	{
		public TransitionalStandardWriter(StandardWriter service) : base(service) { }

		public void WriteLine(string message)
		{
			Service.WriteLine(message);
		}
	}
}
