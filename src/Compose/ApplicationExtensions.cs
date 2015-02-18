using Microsoft.Framework.DependencyInjection;
using System;

namespace Compose
{
    public static class ApplicationExtensions
    {
		public static void UseServices(this Application app, Action<IServiceCollection> configureServices)
		{
			app.UseServices(services =>
			{
				configureServices(services);
				return app.createProvider(services);
			});
		}

		public static void UseServices(this Application app, Func<IServiceCollection, IServiceProvider> configureServices)
		{
			app.HostingServices = configureServices(app.services);
		}
	}
}