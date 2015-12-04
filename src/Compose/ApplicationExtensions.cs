using System;
using Microsoft.Extensions.DependencyInjection;

namespace Compose
{
    public static class ApplicationExtensions
    {
		public static void UseServices(this Application app, Action<IServiceCollection> configureServices)
		{
			configureServices(app.Services = new ServiceCollection());
			app.ApplicationServices = app.Services.BuildServiceProvider();
		}

		public static void UseServices(this Application app, Func<IServiceCollection, IServiceProvider> configureServices)
		{
			app.ApplicationServices = configureServices(app.Services = new ServiceCollection());
		}
    }
}
