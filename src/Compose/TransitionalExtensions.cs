using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Compose
{
	internal static class TransitionalExtensions
	{
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
    }
}
