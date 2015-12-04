namespace Compose.Tests.Fake
{
    public class Consumer
    {
	    public Service Service { get; }

	    public Consumer(Service service)
	    {
		    Service = service;
	    }
    }
}
