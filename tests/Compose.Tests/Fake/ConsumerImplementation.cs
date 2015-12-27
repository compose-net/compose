namespace Compose.Tests.Fake
{
	public class ConsumerImplementation : Consumer
	{
		public Service Service { get; }

		public ConsumerImplementation(Service service)
		{
			Service = service;
		}
	}
}
