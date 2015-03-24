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

		internal static bool IsTransitionMarker(this IServiceDescriptor descriptor)
		{
			return typeof(TransitionMarker).IsAssignableFrom(descriptor.ImplementationType);
		}

		internal static bool ContainsTransitions(this IServiceCollection services)
		{
			return services.Any(x => x.IsTransition());
		}

		internal static bool IsTransition(this IServiceDescriptor descriptor)
		{
			return typeof(ITransition<>).IsAssignableFromGeneric(descriptor.ImplementationType) && descriptor.Lifecycle == LifecycleKind.Singleton;
        }

		internal static IEnumerable<IServiceDescriptor> WithSelfBoundTransitionals(this IEnumerable<IServiceDescriptor> services)
		{
			return services.Where(x => !x.IsTransition())
				.Union(services.Where(x => x.IsTransition()).Select(x => x.SelfBind()));
		}

		internal static IServiceDescriptor SelfBind(this IServiceDescriptor transitional)
		{
			return new ServiceDescriptor(transitional.ImplementationType, transitional.ImplementationType, transitional.Lifecycle);
		}

		internal static Dictionary<Type, Type> GetTransitionalRedirects(this Application app, IServiceCollection services)
		{
			if (services.Any(x => x.ImplementationType == typeof(TransitionMarker)))
				return services.BeforeMarker().Where(x => x.ServiceType.IsInterface).ToDictionary(x => x.ServiceType, x => app.CreateProxy(x.ServiceType));
			return services.Where(x => typeof(TransitionMarker<>).IsAssignableFromGeneric(x.ServiceType))
				.Select(x => x.ServiceType.GetGenericArguments().Single()).ToList().ToDictionary(x => x, x => app.CreateProxy(x));
		}

		internal static IEnumerable<IServiceDescriptor> BeforeMarker(this IEnumerable<IServiceDescriptor> source)
		{
			return source.Take(source.Select((x, i) => new { x, i, }).Last(x => x.x.ImplementationType == typeof(TransitionMarker)).i);
		}
    }
}
