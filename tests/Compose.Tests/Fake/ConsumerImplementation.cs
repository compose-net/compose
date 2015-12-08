namespace Compose.Tests.Fake
{
	public class ConsumerImplementation
	{
		public Service Service { get; }

		public ConsumerImplementation(Service service)
		{
			Service = service;
		}
	}
}
