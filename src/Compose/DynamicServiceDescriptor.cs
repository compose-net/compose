using System;
using Microsoft.Extensions.DependencyInjection;

namespace Compose
{
	internal sealed class DynamicServiceDescriptor : ServiceDescriptor
	{
		private DynamicServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
			: base(serviceType, implementationType, lifetime) { }
		private DynamicServiceDescriptor(Type serviceType, object instance)
			: base(serviceType, instance) { }
		private DynamicServiceDescriptor(Type serviceType, Func<IServiceProvider, object> factory, ServiceLifetime lifetime)
			: base(serviceType, factory, lifetime) { }

		internal static ServiceDescriptor For(ServiceDescriptor serviceDescriptor)
		{
			if (serviceDescriptor.ImplementationType != null)
				return new DynamicServiceDescriptor(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType, serviceDescriptor.Lifetime);
			if (serviceDescriptor.ImplementationInstance != null)
				return new DynamicServiceDescriptor(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationInstance);
			return new DynamicServiceDescriptor(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationFactory, serviceDescriptor.Lifetime);
		}
	}
}
