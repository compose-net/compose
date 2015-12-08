using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Compose
{
	public static class TertiaryProviderExtensions
	{
		public static TertiaryProvider<Service> UseProvider<Service>(this Application app, Action<IServiceCollection> configureServices)
		{
			var serviceDescriptor = app.Services?.SingleOrDefault(x => x.ServiceType == typeof(Service));
			var provider = app.BuildProvider<Service>(serviceDescriptor);
			provider.UseServices(configureServices);
			app.ApplyTransition(serviceDescriptor);
			return provider;
		}

		public static TertiaryProvider<Service> UseProvider<Service>(this Application app, Func<IServiceCollection, IServiceProvider> configureServices)
		{
			var serviceDescriptor = app.Services?.SingleOrDefault(x => x.ServiceType == typeof(Service));
			var provider = app.BuildProvider<Service>(serviceDescriptor);
			provider.UseServices(configureServices);
			app.ApplyTransition(serviceDescriptor);
			return provider;
		}

		private static TertiaryProvider<Service> BuildProvider<Service>(this Application app, ServiceDescriptor serviceDescriptor)
		{
			var serviceType = typeof(Service);
			if (serviceDescriptor == null)
				throw new InvalidOperationException($"No service has been registered for {serviceType.FullName} so it cannot be transitioned.");
			if (!serviceType.GetTypeInfo().IsInterface)
				throw new InvalidOperationException("Only interfaces can be added as tertiary providers.");
			var provider = new TertiaryProvider<Service> { Services = app.Services.ForTertiaryProvider(() => app.ApplicationServices) };
			return provider;
		}

		public static void Transition<Service>(this Application app, TertiaryProvider<Service> provider)
		{
			var transitional = app.ApplicationServices.GetRequiredService<TransitionManager<Service>>();
			if (transitional == null)
				throw new InvalidOperationException($"No provider has been registered for {typeof(Service).Name}. => application.UseProvider<{typeof(Service).Name}>(services => services.AddYourProvider())");
			transitional.Change(() => provider.ApplicationServices.GetRequiredService<Service>());
		}

		private static IServiceCollection ForTertiaryProvider(this IServiceCollection services, Func<IServiceProvider> providerFactory)
			=> new TertiaryServiceCollection(providerFactory, services);

		private static void ApplyTransition(this Application app, ServiceDescriptor original)
		{
			if (original.ImplementationType != null)
				app.ApplyTypeTransition(original);
			else if (original.ImplementationInstance != null)
				app.ApplyInstanceTransition(original);
			else if (original.ImplementationFactory != null)
				app.ApplyFactoryTransition(original);
		}

		private static readonly Type TransitionManager = typeof(TransitionManager<>);
		private static readonly Type DynamicRegister = typeof(DynamicRegister<>);
		private static readonly Type DynamicContainer = typeof(DynamicManagerContainer<,>);
		private static readonly Type FactoryInterface = typeof(AbstractFactory<>);
		private static readonly Type FactoryImplementation = typeof(DelegateAbstractFactory<>);
		private static readonly Type DynamicManagerInterface = typeof(DynamicManager<,>);
		private static readonly Type DynamicManagerImplementation = typeof(WeakReferencingDynamicManager<,>);


		private static void ApplyTypeTransition(this Application app, ServiceDescriptor original)
		{
			app.Services.Add(new ServiceDescriptor(original.ImplementationType, original.ImplementationType, original.Lifetime));
			var dynamicManagerInterface = DynamicManagerInterface.MakeGenericType(original.ServiceType, original.ImplementationType);
			var dynamicManagerImplemenation = DynamicManagerImplementation.MakeGenericType(original.ServiceType, original.ImplementationType);
			app.Services.Add(new ServiceDescriptor(dynamicManagerInterface, dynamicManagerImplemenation, original.Lifetime));
			app.ApplyTransition(original, dynamicManagerImplemenation);
		}

		private static void ApplyInstanceTransition(this Application app, ServiceDescriptor original)
		{
			var implementationType = original.ImplementationInstance.GetType();
			app.Services.Add(new ServiceDescriptor(implementationType, original.ImplementationInstance));
			var dynamicManagerInterface = DynamicManagerInterface.MakeGenericType(original.ServiceType, implementationType);
			var dynamicManagerImplemenation = DynamicManagerImplementation.MakeGenericType(original.ServiceType, implementationType);
			app.Services.Add(new ServiceDescriptor(dynamicManagerInterface, dynamicManagerImplemenation, original.Lifetime));
			app.ApplyTransition(original, dynamicManagerImplemenation);
		}

		private static void ApplyFactoryTransition(this Application app, ServiceDescriptor original)
		{
			var implementationFactoryInterfaceType = FactoryInterface.MakeGenericType(original.ServiceType);
			var implementationFactoryImplementationType = FactoryImplementation.MakeGenericType(original.ServiceType);
			Func<IServiceProvider, object> implementationFactory =
				provider => Activator.CreateInstance(implementationFactoryImplementationType,
					(Func<object>)(() => original.ImplementationFactory(provider))
				);
			app.Services.Add(new ServiceDescriptor(implementationFactoryInterfaceType, implementationFactory, original.Lifetime));
			var dynamicManagerInterface = DynamicManagerInterface.MakeGenericType(original.ServiceType, original.ServiceType);
			Func<IServiceProvider, object> dynamicManagerFactory =
				provider => DynamicManagerFactory.ForFactory(dynamicManagerInterface.GetTypeInfo(),
					provider.GetRequiredService(DynamicContainer.MakeGenericType(original.ServiceType, original.ServiceType)),
					provider.GetRequiredService<TransitionManagerContainer>(),
					provider.GetRequiredService(implementationFactoryInterfaceType)
				);
			app.Services.Add(new ServiceDescriptor(dynamicManagerInterface, dynamicManagerFactory, original.Lifetime));
			app.Services.Add(new ServiceDescriptor(TransitionManager.MakeGenericType(original.ServiceType), dynamicManagerFactory, original.Lifetime));
			var dynamicRegister = DynamicRegister.MakeGenericType(original.ServiceType);
			app.Services.Add(new ServiceDescriptor(dynamicRegister, dynamicManagerFactory, original.Lifetime));
			app.Services.Replace(new ServiceDescriptor(
				original.ServiceType,
				provider => new DynamicActivator(
					() => app.CreateProxy(original.ServiceType.GetTypeInfo()),
					() => provider.GetService(dynamicRegister)
				).Create(),
				original.Lifetime)
			);
		}

		private static void ApplyTransition(this Application app, ServiceDescriptor original, Type dynamicManagerType)
		{
			app.Services.Add(new ServiceDescriptor(TransitionManager.MakeGenericType(original.ServiceType), dynamicManagerType, original.Lifetime));
			var dynamicRegister = DynamicRegister.MakeGenericType(original.ServiceType);
			app.Services.Add(new ServiceDescriptor(dynamicRegister, dynamicManagerType, original.Lifetime));
			app.Services.Replace(new ServiceDescriptor(
				original.ServiceType, 
				provider => new DynamicActivator(
					() => app.CreateProxy(original.ServiceType.GetTypeInfo()), 
					() => provider.GetService(dynamicRegister)
				).Create(), 
				original.Lifetime)
			);
		}
	}
}
