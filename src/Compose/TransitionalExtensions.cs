using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Compose
{
	internal static class TransitionalExtensions
	{
		internal static bool ContainsTransitionMarkers(this IServiceCollection services)
		{
			return services.Any(x => x.IsTransitionMarker());
		}

		internal static bool IsTransitionMarker(this ServiceDescriptor descriptor)
		{
			return typeof(TransitionMarker).IsAssignableFrom(descriptor.ImplementationType);
		}

		internal static bool ContainsTransitions(this ServiceCollection services)
		{
			return services.Any(x => x.IsTransition());
		}

		internal static bool IsTransition(this ServiceDescriptor descriptor)
		{
			return typeof(ITransition<>).IsAssignableFromGeneric(descriptor.ImplementationType) && descriptor.Lifetime == ServiceLifetime.Singleton;
        }

		internal static IEnumerable<ServiceDescriptor> WithSelfBoundTransitionals(this IEnumerable<ServiceDescriptor> services)
		{
			return services.Where(x => !x.IsTransition())
				.Union(services.Where(x => x.IsTransition()).Select(x => x.SelfBind()));
		}

		internal static ServiceDescriptor SelfBind(this ServiceDescriptor transitional)
		{
			return new ServiceDescriptor(transitional.ImplementationType, transitional.ImplementationType, transitional.Lifetime);
		}

		internal static Dictionary<Type, Type> GetTransitionalRedirects(this Application app, IServiceCollection services)
		{
			if (services.Any(x => x.ImplementationType == typeof(TransitionMarker)))
				return services.BeforeMarker().Where(x => x.ServiceType.IsInterface).ToList().ToDictionary(x => x.ServiceType, x => app.CreateProxy(x.ServiceType));
			return services.Where(x => typeof(TransitionMarker<>).IsAssignableFromGeneric(x.ServiceType))
				.Select(x => x.ServiceType.GetGenericArguments().Single()).ToList().ToDictionary(x => x, x => app.CreateProxy(x));
		}

		internal static IEnumerable<ServiceDescriptor> BeforeMarker(this IEnumerable<ServiceDescriptor> source)
		{
			return source.Take(source.Select((x, i) => new { x, i, }).Last(x => x.x.ImplementationType == typeof(TransitionMarker)).i);
		}
    }
}
