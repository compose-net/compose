using System;
using Microsoft.Extensions.DependencyInjection;

namespace Compose
{
    public static class TertiaryProviderExtensions
    {
	    public static TertiaryProvider<Service> UseProvider<Service>(this Application app, Action<IServiceCollection> configureServices)
	    {
		    var provider = new TertiaryProvider<Service> { Services = app.Services.ForTertiaryProvider(app.ApplicationServices) };
		    provider.UseServices(configureServices);
		    return provider;
	    }

		public static TertiaryProvider<Service> UseProvider<Service>(this Application app, Func<IServiceCollection, IServiceProvider> configureServices)
		{
			var provider = new TertiaryProvider<Service> { Services = app.Services.ForTertiaryProvider(app.ApplicationServices) };
			provider.UseServices(configureServices);
			return provider;
		}

	    private static IServiceCollection ForTertiaryProvider(this IServiceCollection services, IServiceProvider provider)
		    => new TertiaryServiceCollection(provider, services);
    }
}
