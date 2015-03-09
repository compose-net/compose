using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Compose
{
	internal sealed class WrappedServiceProvider : ISingletonRepositoryServiceProvider
	{
		private readonly IServiceCollection _services;
		private Dictionary<Type, object> _singletons;
		private IServiceProvider _fallback;
		private IServiceProvider _snapshot;

		public WrappedServiceProvider(IServiceCollection services)
		{
			_singletons = services.Where(x => x.Lifecycle == LifecycleKind.Singleton)
				.ToDictionary(x => x.ImplementationType, x => (object)null);
			_fallback = CreateFallbackProvider(services);
			_services = services;
        }

		public object GetService(Type serviceType)
		{
			if (_singletons.ContainsKey(serviceType))
				return ResolveSingleton(serviceType);
			return _fallback.GetService(serviceType);
		}

		public void Extend(ServiceDescriptor service)
		{
			_services.Add(service);
			_fallback = CreateFallbackProvider(_services);
		}

		public void AppendSingleton(Type serviceType)
		{
			if (!_singletons.ContainsKey(serviceType))
				_singletons.Add(serviceType, null);
		}

		private IServiceProvider CreateFallbackProvider(IServiceCollection services)
		{
			return (IServiceProvider)Activator.CreateInstance(Constants.GetServiceProvider(), services);
		}

		private object ResolveSingleton(Type serviceType)
		{
			if (_singletons[serviceType] == null)
				_singletons[serviceType] = _fallback.GetService(serviceType);
			return _singletons[serviceType];
		}

		public void Snapshot()
		{
			_snapshot = CreateFallbackProvider(_services);
		}

		public void Restore()
		{
			_fallback = _snapshot ?? _fallback;
		}
	}
}
