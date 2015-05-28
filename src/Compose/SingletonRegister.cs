using Microsoft.Framework.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Compose
{
    internal sealed class SingletonRegister
    {
		private readonly IServiceCollection _services;

		public SingletonRegister(IServiceCollection services)
		{
			_services = services;
		}

		public bool CanResolveSingleton(Type serviceType)
		{
			return _services.Any(x => x.Lifetime == ServiceLifetime.Singleton && serviceType.IsAssignableFrom(x.ServiceType));
		}

		public object Resolve(IServiceProvider provider, Type serviceType)
		{
			var descriptor = _services.BestSingletonMatchFor(serviceType);
			if (descriptor == null || descriptor.Lifetime != ServiceLifetime.Singleton)
				throw new InvalidOperationException($"Cannot resolve {serviceType.FullName} as a singleton.");
			if (descriptor.HasBeenRegistered())
				return descriptor.Resolve(provider);
			descriptor = descriptor.Register(provider, _services);
			return descriptor.Resolve(provider);
        }
    }
}
