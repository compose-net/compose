using Microsoft.Framework.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Compose
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddTransitional<TService, TImplementation>(this IServiceCollection services) where TImplementation : TService
		{
			return services
				.AddTransient<TService, TImplementation>()
				.WithTransitional<TService>();
		}

		public static IServiceCollection AsTransitional(this IServiceCollection services)
		{
			return services.AddTransient<TransitionMarker>();
		}

		public static IServiceCollection WithTransitional<TService>(this IServiceCollection services)
		{
			return services.AddTransient<TransitionMarker<TService>>();
		}
		
		internal static SingletonRegister BuildSingletonRegister(this IServiceCollection services)
		{
			return new SingletonRegister(services);
		}

		internal static ServiceDescriptor BestSingletonMatchFor(this IServiceCollection services, Type serviceType)
		{
            return services.Where(x => x.Lifetime == ServiceLifetime.Singleton && serviceType.IsAssignableFrom(x.ServiceType))
				.OrderByDescending(x => x.ServiceType == serviceType)
				.LastOrDefault();
		}
	}
}
