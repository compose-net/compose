using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Compose
{
	public static class ServiceProviderExtensions
	{
		public static void UseServices(this Application app, Action<IServiceCollection> configureServices)
			=> app.ConfigureDynamicEmission().UseBuiltInProvider(configureServices);

		internal static void UseServices<Service>(this TertiaryProvider<Service> app, Action<IServiceCollection> configureServices)
			=> app.UseBuiltInProvider(configureServices);

		private static void UseBuiltInProvider(this ServiceProvider app, Action<IServiceCollection> configureServices)
		{
			configureServices(app.Services ?? (app.Services = new ServiceCollection()));
			app.ApplicationServiceFactory = () => app.Services.BuildServiceProvider();
		}

		public static void UseServices(this Application app, Func<IServiceCollection, IServiceProvider> configureServices)
			=> app.ConfigureDynamicEmission().UseCustomProvider(configureServices);

		internal static void UseServices<Service>(this TertiaryProvider<Service> app, Func<IServiceCollection, IServiceProvider> configureServices)
			=> app.UseCustomProvider(configureServices);

		private static void UseCustomProvider(this ServiceProvider app, Func<IServiceCollection, IServiceProvider> configureServices)
			=> app.ApplicationServiceFactory = () => configureServices(app.Services ?? (app.Services = new ServiceCollection()));

		private static Application ConfigureDynamicEmission(this Application app)
		{
			app.Services = app.Services ?? new ServiceCollection();
			DynamicEmissionDescriptors.ForEach(app.Services.TryAdd);
			return app;
		}

		internal static readonly List<ServiceDescriptor> DynamicEmissionDescriptors
			= new List<ServiceDescriptor>
			{
				ServiceDescriptor.Singleton<DynamicEmitter, IlGeneratingDynamicEmitter>(),
				ServiceDescriptor.Singleton(typeof(DynamicManagerContainer<,>), typeof(SyncLockDynamicManagerContainer<,>)),
				ServiceDescriptor.Singleton<TransitionManagerContainer, ConcurrentTransitionManagerContainer>()
			};
	}
}
