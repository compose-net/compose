using Microsoft.Extensions.DependencyInjection;

namespace Compose
{
	public class Application : ServiceProvider
	{
		protected internal virtual void PreServiceConfiguration(IServiceCollection services) { }
	}
}
