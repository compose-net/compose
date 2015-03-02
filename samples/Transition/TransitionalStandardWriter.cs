namespace Transition
{
	internal class TransitionalStandardWriter : Compose.DirectDefaultTransition<IWriter, StandardWriter>, IWriter
	{
		public TransitionalStandardWriter(StandardWriter service) : base(service) { }

		public void WriteLine(string message)
		{
			Service.WriteLine(message);
		}
	}
}
