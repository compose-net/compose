using Microsoft.Framework.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Compose
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddTransitional(this IServiceCollection services, Type serviceType, Type implementationType)
			=> services.AddTransient(serviceType, implementationType).WithTransitional(serviceType);

		public static IServiceCollection AddTransitional<TService, TImplementation>(this IServiceCollection services) where TImplementation : TService
			=> services.AddTransient<TService, TImplementation>().WithTransitional<TService>();

		public static IServiceCollection AsTransitional(this IServiceCollection services)
			=> services.AddTransient<TransitionMarker>();

		public static IServiceCollection WithTransitional(this IServiceCollection services, Type serviceType)
			=> services.AddTransient(typeof(TransitionMarker<>).MakeGenericType(new[] { serviceType }));

		public static IServiceCollection WithTransitional<TService>(this IServiceCollection services)
			=> services.AddTransient<TransitionMarker<TService>>();
		
		internal static SingletonRegister BuildSingletonRegister(this IServiceCollection services)
			=> new SingletonRegister(services);

		internal static ServiceDescriptor BestSingletonMatchFor(this IServiceCollection services, Type serviceType)
			=> services.Where(x => x.Lifetime == ServiceLifetime.Singleton && serviceType.IsAssignableFrom(x.ServiceType))
				.OrderByDescending(x => x.ServiceType == serviceType)
				.LastOrDefault();
	}
}
