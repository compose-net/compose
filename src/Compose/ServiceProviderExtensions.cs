using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
				.UseBuiltInProvider(configureServices);

		private static Application RunPreConfiguration(this Application app)
		{
			app.PreServiceConfiguration(app.Services ?? (app.Services = new ServiceCollection()));
			return app;
		}

		internal static void UseServices<Service>(this TertiaryProvider<Service> app, Action<IServiceCollection> configureServices)
			=> app.UseBuiltInProvider(configureServices);

		private static void UseBuiltInProvider(this Application app, Action<IServiceCollection> configureServices)
		{
			configureServices(app.Services ?? (app.Services = new ServiceCollection()));
			app.ApplicationServiceFactory = () =>
			{
				var provider = app.Services.BuildServiceProvider();
				app.PostServiceConfiguration(app.Services.ToList().AsReadOnly());
				return provider;
			};
		}

		public static void UseServices(this Application app, Func<IServiceCollection, IServiceProvider> configureServices)
			=> app
				.ConfigureDynamicEmission()
				.RunPreConfiguration()
				.UseCustomProvider(configureServices);

		internal static void UseServices<Service>(this TertiaryProvider<Service> app, Func<IServiceCollection, IServiceProvider> configureServices)
			=> app.UseCustomProvider(configureServices);

		private static void UseCustomProvider(this Application app, Func<IServiceCollection, IServiceProvider> configureServices)
			=> app.ApplicationServiceFactory = () =>
			{
				var provider = configureServices(app.Services ?? (app.Services = new ServiceCollection()));
				app.PostServiceConfiguration(app.Services.ToList().AsReadOnly());
				return provider;
			};

		private static Application ConfigureDynamicEmission(this Application app)
		{
			app.Services = app.Services ?? new ServiceCollection();
			DynamicEmissionDescriptors.ForEach(app.Services.TryAdd);
			return app;
		}

		private static readonly List<ServiceDescriptor> DynamicEmissionDescriptors
			= new List<ServiceDescriptor>
			{
				ServiceDescriptor.Singleton<DynamicFactory, ConcurrentCachingDynamicFactory>().WithDynamicMarker(),
				ServiceDescriptor.Singleton<DynamicEmitter, IlGeneratingDynamicEmitter>().WithDynamicMarker(),
				ServiceDescriptor.Singleton(KnownTypes.OpenDynamicManagerContainer, KnownTypes.OpenDynamicManagerContainerImplementation).WithDynamicMarker(),
				ServiceDescriptor.Singleton<TransitionManagerContainer, ConcurrentTransitionManagerContainer>().WithDynamicMarker()
			};

		internal static IEnumerable<ServiceDescriptor> ExcludingDynamicServices(this IServiceCollection services)
			=> services.Where(x => !x.IsDynamicService());

		internal static ServiceDescriptor WithDynamicMarker(this ServiceDescriptor serviceDescriptor)
			=> DynamicServiceDescriptor.For(serviceDescriptor);

		private static bool IsDynamicService(this ServiceDescriptor serviceDescriptor)
			=> serviceDescriptor is DynamicServiceDescriptor;
	}
}
