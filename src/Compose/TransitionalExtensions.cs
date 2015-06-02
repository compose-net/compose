using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Compose
{
	internal static class TransitionalExtensions
	{
		internal static bool ContainsTransitionMarkers(this Application app)
		{
			return app.Services.Any(x => x.IsTransitionMarker());
		}

		internal static bool IsTransitionMarker(this ServiceDescriptor descriptor)
		{
			return typeof(TransitionMarker).IsAssignableFrom(descriptor.ImplementationType);
		}

		internal static void ApplyTransitions(this Application app)
		{
			var transitionalServices = app.Services
				.GetTransitionalServices()
#if DEBUG
				.ToList();
#endif
			if (!transitionalServices.Any()) return;
			app.Services.TryAdd(ServiceDescriptor.Singleton(typeof(IDynamicManagerContainer<,>), typeof(SyncLockDynamicManagerContainer<,>)));
			app.Services.TryAdd(ServiceDescriptor.Singleton<ITransitionManagerContainer, ConcurrentTransitionManagerContainer>());
            foreach (var transitionalService in transitionalServices)
				app.Services.ApplyTransition(app, transitionalService);
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

		private static Type TransitionManager = typeof(ITransitionManager<>);
		private static Type DynamicRegister = typeof(IDynamicRegister<>);
		private static Type DynamicContainer = typeof(IDynamicManagerContainer<,>);
		private static Type FactoryInterface = typeof(IAbstractFactory<>);
		private static Type FactoryImplementation = typeof(LambdaAbstractFactory<>);
		private static Type DynamicManagerInterface = typeof(IDynamicManager<,>);
		private static Type DynamicManagerImplementation = typeof(DynamicManager<,>);


		private static void ApplyTypeTransition(this IServiceCollection services, Application app, ServiceDescriptor original)
		{
			services.Add(new ServiceDescriptor(original.ImplementationType, original.ImplementationType, original.Lifetime));
			var dynamicManagerInterface = DynamicManagerInterface.MakeGenericType(original.ServiceType, original.ImplementationType);
			var dynamicManagerImplemenation = DynamicManagerImplementation.MakeGenericType(original.ServiceType, original.ImplementationType);
			services.Add(new ServiceDescriptor(dynamicManagerInterface, dynamicManagerImplemenation, original.Lifetime));
			services.ApplyTransition(app, original, dynamicManagerImplemenation);
		}

		private static void ApplyInstanceTransition(this IServiceCollection services, Application app, ServiceDescriptor original)
		{
			var implementationType = original.ImplementationInstance.GetType();
            services.Add(new ServiceDescriptor(implementationType, original.ImplementationInstance));
			var dynamicManagerInterface = DynamicManagerInterface.MakeGenericType(original.ServiceType, implementationType);
			var dynamicManagerImplemenation = DynamicManagerImplementation.MakeGenericType(original.ServiceType, implementationType);
			services.Add(new ServiceDescriptor(dynamicManagerInterface, dynamicManagerImplemenation, original.Lifetime));
			services.ApplyTransition(app, original, dynamicManagerImplemenation);
		}

		private static void ApplyFactoryTransition(this IServiceCollection services, Application app, ServiceDescriptor original)
		{
			var implementationFactoryInterfaceType = FactoryInterface.MakeGenericType(original.ServiceType);
			var implementationFactoryImplementationType = FactoryImplementation.MakeGenericType(original.ServiceType);
			Func<IServiceProvider, object> implementationFactory = 
				provider => Activator.CreateInstance(implementationFactoryImplementationType, 
					(Func<object>)(() => original.ImplementationFactory(provider))
				);
            services.Add(new ServiceDescriptor(implementationFactoryInterfaceType, implementationFactory, original.Lifetime));
			var dynamicManagerInterface = DynamicManagerInterface.MakeGenericType(original.ServiceType, original.ServiceType);
			Func<IServiceProvider, object> dynamicManagerFactory =
				provider => DynamicManagerFactory.ForFactory(dynamicManagerInterface.GetTypeInfo(),
                    provider.GetRequiredService(DynamicContainer.MakeGenericType(original.ServiceType, original.ServiceType)),
					provider.GetRequiredService<ITransitionManagerContainer>(),
					provider.GetRequiredService(implementationFactoryInterfaceType)
				);
			services.Add(new ServiceDescriptor(dynamicManagerInterface, dynamicManagerFactory, original.Lifetime));
			services.Add(new ServiceDescriptor(TransitionManager.MakeGenericType(original.ServiceType), dynamicManagerFactory, original.Lifetime));
			services.Add(new ServiceDescriptor(DynamicRegister.MakeGenericType(original.ServiceType), dynamicManagerFactory, original.Lifetime));
			var dynamicProxyType = app.CreateProxy(original.ServiceType.GetTypeInfo());
			services.Replace(new ServiceDescriptor(original.ServiceType, dynamicProxyType, original.Lifetime));
		}

		private static void ApplyTransition(this IServiceCollection services, Application app, ServiceDescriptor original, Type dynamicManagerType)
		{
			services.Add(new ServiceDescriptor(TransitionManager.MakeGenericType(original.ServiceType), dynamicManagerType, original.Lifetime));
			services.Add(new ServiceDescriptor(DynamicRegister.MakeGenericType(original.ServiceType), dynamicManagerType, original.Lifetime));
			var dynamicProxyType = app.CreateProxy(original.ServiceType.GetTypeInfo());
			services.Replace(new ServiceDescriptor(original.ServiceType, dynamicProxyType, original.Lifetime));
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
				.Where(service => service.ServiceType.GetTypeInfo().IsInterface)
				.ToList();
		}

		private static IEnumerable<ServiceDescriptor> GetTargettedTransitionalServices(this IServiceCollection services)
		{
			var targettedMarkerTypeInfo = typeof(TransitionMarker<>).GetTypeInfo(); // depicts a single service to be transitional
			var targettedTransitionalMarkers = services
				.Where(marker => targettedMarkerTypeInfo.IsAssignableFromGeneric(marker.ServiceType.GetTypeInfo()))
				.Select(marker => marker.ServiceType.GetGenericArguments().Single());
			return services.Where(service => targettedTransitionalMarkers.Contains(service.ServiceType));
		}

		private static IEnumerable<ServiceDescriptor> GetBlanketTransitionalServices(this IServiceCollection services, Type blanketMarkerType)
		{
			return services.BeforeLast(blanketMarkerType);
        }

		internal static IEnumerable<ServiceDescriptor> BeforeLast(this IEnumerable<ServiceDescriptor> source, Type transitionMarkerType)
		{
			var lastMarkerIndex = source.Select((x, i) => new { x, i, }).LastOrDefault(x => x.x.ImplementationType == transitionMarkerType)?.i;
			if (!lastMarkerIndex.HasValue)
				return Enumerable.Empty<ServiceDescriptor>();
            return source.Take(lastMarkerIndex.Value);
		}
    }
}
