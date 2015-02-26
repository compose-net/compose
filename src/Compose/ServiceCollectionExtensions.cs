using Microsoft.Framework.DependencyInjection;
using System;

namespace Compose
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddTransitional<TService, TImplementation>(this IServiceCollection services) 
			where TImplementation : ITransition<TService>, TService
		{
			if (typeof(DefaultTransition<,>).IsAssignableFromGeneric(typeof(TImplementation)))
				services.AddDefault<TService, TImplementation>();
			return services
				.AddSingleton<TService, TImplementation>()
				.AddTransient<ITransition<TService>, TImplementation>();
		}

		internal static IServiceCollection AddDefault<TService, TImplementation>(this IServiceCollection services)
		{
			var defaultService = typeof(TImplementation).GetDefaultType();
			return services.AddTransient(defaultService, defaultService);
		}

		internal static Type GetDefaultType(this Type givenType)
		{
			if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == typeof(DefaultTransition<,>))
				return givenType.GetGenericArguments()[1];
			return givenType.BaseType.GetDefaultType();
		}

		internal static bool IsAssignableFromGeneric(this Type genericType, Type givenType)
		{
			// Don't need interface checking for the above
			//foreach (var it in givenType.GetInterfaces())
			//	if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
			//		return true;

			if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
				return true;

			Type baseType = givenType.BaseType;
			if (baseType == null) return false;

			return genericType.IsAssignableFromGeneric(baseType);
		}
	}
}
