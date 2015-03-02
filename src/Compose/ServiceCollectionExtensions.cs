using Microsoft.Framework.DependencyInjection;
using System;

namespace Compose
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddTransitional<TService, TImplementation>(this IServiceCollection services) 
			where TImplementation : TService
		{
			if (typeof(DefaultDirectTransition<,>).IsAssignableFromGeneric(typeof(TImplementation)))
				services.AddDefault<TService, TImplementation>();
			else
				services.AddTransient<TImplementation, TImplementation>();
            return services
				.AddSingleton<TService, TImplementation>()
				.AddSingleton<IFactory<TService>, DirectFactoryTransition<TService, TImplementation>>(); ;
		}

		public static IServiceCollection WithTransitional<TService, TImplementation>(this IServiceCollection services)
			where TImplementation : TService, ITransition<TService>
		{
			return services
				.AddTransient<ITransition<TService>, TImplementation>()
				.AddTransient<TransitionalMarker, TransitionalMarker>();
		}

		internal static IServiceCollection AddDefault<TService, TImplementation>(this IServiceCollection services)
		{
			var defaultService = typeof(TImplementation).GetDefaultType();
			return services.AddTransient(defaultService, defaultService);
		}

		internal static Type GetDefaultType(this Type givenType)
		{
			if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == typeof(DefaultDirectTransition<,>))
				return givenType.GetGenericArguments()[1];
			return givenType.BaseType.GetDefaultType();
		}
	}
}
