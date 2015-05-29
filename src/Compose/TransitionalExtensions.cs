using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
			return typeof(ITransition<>).GetTypeInfo()
				.IsAssignableFromGeneric(descriptor.ImplementationType.GetTypeInfo()) && descriptor.Lifetime == ServiceLifetime.Singleton;
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

		internal static void ApplyTransitions(this Application app, IServiceCollection services)
		{
			var transitionalServices = services
				.GetTransitionalServices()
#if DEBUG
				.ToList();
#endif
			foreach (var transitionalService in transitionalServices)
				services.ApplyTransition(transitionalService);
		}

		private static void ApplyTransition(this IServiceCollection services, ServiceDescriptor original)
		{

		}

		private static IEnumerable<ServiceDescriptor> GetTransitionalServices(this IServiceCollection services)
		{
			var blanketMarkerType = typeof(TransitionMarker); // depicts all previous services in collection to be transitional
			return services
				.GetTargettedTransitionalServices().Union(
					services.GetBlanketTransitionalServices(blanketMarkerType)
				)
				.Distinct()
				.Where(service => !blanketMarkerType.IsAssignableFrom(service.ServiceType))
				.ToList();
		}

		private static IEnumerable<ServiceDescriptor> GetTargettedTransitionalServices(this IServiceCollection services)
		{
			var targettedMarkerTypeInfo = typeof(TransitionMarker<>).GetTypeInfo(); // depicts a single service to be transitional
			var targettedTransitionalMarkers = services
				.Where(marker => targettedMarkerTypeInfo.IsAssignableFrom(marker.ServiceType.GetTypeInfo()))
				.Select(marker => marker.ServiceType.GetGenericArguments().Single());
			return services.Where(service => targettedTransitionalMarkers.Contains(service.ServiceType));
		}

		private static IEnumerable<ServiceDescriptor> GetBlanketTransitionalServices(this IServiceCollection services, Type blanketMarkerType)
		{
			return services.BeforeLast(blanketMarkerType);
        }

		internal static IEnumerable<ServiceDescriptor> BeforeLast(this IEnumerable<ServiceDescriptor> source, Type transitionMarkerType)
		{
			return source.Take(source.Select((x, i) => new { x, i, }).Last(x => x.x.ImplementationType == transitionMarkerType).i);
		}
    }
}
