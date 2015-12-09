using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Compose
{
	public static class ServiceProviderExtensions
	{
		public static void UseServices(this Application app, Action<IServiceCollection> configureServices)
			=> app
			.ConfigureDynamicEmission()
			.RunPreConfiguration()
			.UseBuiltInProvider(configureServices)
			.RunPostConfiguration();

		private static Application RunPreConfiguration(this Application app)
		{
			app.PreServiceConfiguration(app.Services ?? (app.Services = new ServiceCollection()));
			return app;
		}

		private static Application RunPostConfiguration(this Application app)
			=> null;

		internal static void UseServices<Service>(this TertiaryProvider<Service> app, Action<IServiceCollection> configureServices)
			=> app.UseBuiltInProvider(configureServices);

		private static Application UseBuiltInProvider(this Application app, Action<IServiceCollection> configureServices)
		{
			configureServices(app.Services ?? (app.Services = new ServiceCollection()));
			app.ApplicationServiceFactory = () => app.Services.BuildServiceProvider();
			return app;
		}

		public static void UseServices(this Application app, Func<IServiceCollection, IServiceProvider> configureServices)
			=> app
			.ConfigureDynamicEmission()
			.RunPreConfiguration()
			.UseCustomProvider(configureServices)
			.RunPostConfiguration();

		internal static void UseServices<Service>(this TertiaryProvider<Service> app, Func<IServiceCollection, IServiceProvider> configureServices)
			=> app.UseCustomProvider(configureServices);

		private static Application UseCustomProvider(this Application app, Func<IServiceCollection, IServiceProvider> configureServices)
		{
			app.ApplicationServiceFactory = () => configureServices(app.Services ?? (app.Services = new ServiceCollection()));
			return app;
		}

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
				ServiceDescriptor.Singleton(KnownTypes.OpenDynamicManagerContainer, KnownTypes.OpenDynamicManagerContainerImplementation),
				ServiceDescriptor.Singleton<TransitionManagerContainer, ConcurrentTransitionManagerContainer>()
			};
	}
}
