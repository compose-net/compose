﻿using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Compose
{
	internal sealed class TertiaryServiceCollection : ServiceCollection
	{
		public TertiaryServiceCollection(Func<IServiceProvider> providerFactory, IServiceCollection services)
		{
			foreach (var serviceDescriptor in services.Except(ServiceProviderExtensions.DynamicEmissionDescriptors) ?? Enumerable.Empty<ServiceDescriptor>())
				this.Add(Describe(serviceDescriptor, providerFactory));
		}

		private ServiceDescriptor Describe(ServiceDescriptor serviceDescriptor, Func<IServiceProvider> providerFactory)
		{
			if (serviceDescriptor.Lifetime == ServiceLifetime.Singleton)
				return ServiceDescriptor.Singleton(serviceDescriptor.ServiceType, _ => providerFactory().GetRequiredService(serviceDescriptor.ServiceType));
			if (serviceDescriptor.ImplementationFactory != null)
				switch (serviceDescriptor.Lifetime)
				{
					case ServiceLifetime.Scoped:
						return ServiceDescriptor.Scoped(serviceDescriptor.ServiceType, _ => providerFactory().GetRequiredService(serviceDescriptor.ServiceType));
					case ServiceLifetime.Singleton:
						return ServiceDescriptor.Singleton(serviceDescriptor.ServiceType, _ => providerFactory().GetRequiredService(serviceDescriptor.ServiceType));
					case ServiceLifetime.Transient:
						return ServiceDescriptor.Transient(serviceDescriptor.ServiceType, _ => providerFactory().GetRequiredService(serviceDescriptor.ServiceType));
				}
			return serviceDescriptor;
		}
	}
}
