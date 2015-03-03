using Microsoft.Framework.DependencyInjection;
using System;

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
	}
}
