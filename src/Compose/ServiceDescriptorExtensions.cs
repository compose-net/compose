using Microsoft.Framework.DependencyInjection;
using System;

namespace Compose
{
    internal static class ServiceDescriptorExtensions
    {
		internal static bool HasBeenRegistered(this ServiceDescriptor descriptor)
		{
			return descriptor.ImplementationInstance != null;
		}

		internal static ServiceDescriptor Register(this ServiceDescriptor descriptor, IServiceProvider provider, IServiceCollection services)
		{
			if (descriptor.ImplementationInstance != null || descriptor.ImplementationFactory != null)
				return descriptor;
			var instance = provider.GetService(descriptor.ServiceType);
			if (instance == null) return descriptor;
            services.Replace(new ServiceDescriptor(descriptor.ServiceType, instance));
			return descriptor;
		}

		internal static object Resolve(this ServiceDescriptor descriptor, IServiceProvider provider)
		{
			return descriptor.ImplementationFactory == null ?
				descriptor.ImplementationInstance : 
				descriptor.ImplementationFactory(provider);
		}
    }
}
