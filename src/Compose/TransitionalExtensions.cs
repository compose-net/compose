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

		internal static void ApplyTransitions(this Application app, IServiceCollection services)
		{
			var transitionalServices = services
				.GetTransitionalServices()
#if DEBUG
				.ToList();
#endif
			foreach (var transitionalService in transitionalServices)
				services.ApplyTransition(app, transitionalService);
		}

		private static void ApplyTransition(this IServiceCollection services, Application app, ServiceDescriptor original)
		{
			if (original.ImplementationType != null)
				services.ApplyTypeTransition(app, original);
			else if (original.ImplementationInstance != null)
				services.ApplyInstanceTransition(app, original);
			else if (original.ImplementationFactory != null)
				services.ApplyFactoryTransition(app, original);
		}

		private static void ApplyTypeTransition(this IServiceCollection services, Application app, ServiceDescriptor original)
		{
			services.Add(new ServiceDescriptor(original.ImplementationType, original.ImplementationType, original.Lifetime));
			var dynamicManagerType = DynamicManagerFactory.ForType(original.ServiceType, original.ImplementationType);
			services.Add(new ServiceDescriptor(dynamicManagerType, dynamicManagerType, original.Lifetime));
			var dynamicProxyType = app.CreateProxy(original.ServiceType.GetTypeInfo(), dynamicManagerType.GetTypeInfo());
			services.Replace(new ServiceDescriptor(original.ServiceType, dynamicProxyType.AsType(), original.Lifetime));
		}

		private static void ApplyInstanceTransition(this IServiceCollection services, Application app, ServiceDescriptor original)
		{
			var dynamicManager = DynamicManagerFactory.ForInstance(original.ServiceType, original.ImplementationInstance);
			var dynamicManagerType = dynamicManager.GetType();
			var dynamicProxyType = app.CreateProxy(original.ServiceType.GetTypeInfo(), dynamicManagerType.GetTypeInfo());
            var dynamicProxy = Activator.CreateInstance(dynamicProxyType, dynamicManager);
			services.Replace(new ServiceDescriptor(original.ServiceType, dynamicProxy));
		}

		private static void ApplyFactoryTransition(this IServiceCollection services, Application app, ServiceDescriptor original)
		{
			var dynamicManagerType = DynamicManagerFactory.ForType(original.ServiceType, original.ServiceType);
			var dynamicProxyType = app.CreateProxy(original.ServiceType.GetTypeInfo(), dynamicManagerType.GetTypeInfo());
			var dynamicFactory = DynamicManagerFactory.ForFactory(original.ImplementationFactory, dynamicManagerType, dynamicProxyType);
            services.Replace(new ServiceDescriptor(original.ServiceType, dynamicFactory, original.Lifetime));
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
