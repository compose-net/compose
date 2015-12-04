using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Compose
{
	internal sealed class TertiaryServiceCollection : ServiceCollection
	{
		public TertiaryServiceCollection(IServiceProvider provider, IServiceCollection services)
		{
			foreach (var serviceDescriptor in services ?? Enumerable.Empty<ServiceDescriptor>())
				this.Add(Describe(serviceDescriptor, provider));
		}

		private ServiceDescriptor Describe(ServiceDescriptor serviceDescriptor, IServiceProvider provider)
		{
			if (serviceDescriptor.Lifetime == ServiceLifetime.Singleton)
				return ServiceDescriptor.Singleton(serviceDescriptor.ServiceType, _ => provider.GetRequiredService(serviceDescriptor.ServiceType));
			if (serviceDescriptor.ImplementationFactory != null)
				switch (serviceDescriptor.Lifetime)
				{
					case ServiceLifetime.Scoped:
						return ServiceDescriptor.Scoped(serviceDescriptor.ServiceType, _ => provider.GetRequiredService(serviceDescriptor.ServiceType));
					case ServiceLifetime.Singleton:
						return ServiceDescriptor.Singleton(serviceDescriptor.ServiceType, _ => provider.GetRequiredService(serviceDescriptor.ServiceType));
					case ServiceLifetime.Transient:
						return ServiceDescriptor.Transient(serviceDescriptor.ServiceType, _ => provider.GetRequiredService(serviceDescriptor.ServiceType));
				}
			return serviceDescriptor;
		}
	}
}
