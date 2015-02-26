using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Compose
{
	internal sealed class WrappedReflectionServiceProvider : IServiceProvider
	{
		private readonly IServiceProvider _underlyingProvider;
		private readonly Dictionary<Type, object> _singletons;

		public WrappedReflectionServiceProvider(WrappedReflectionServiceProvider provider, IEnumerable<IServiceDescriptor> services)
		{
			_underlyingProvider = (IServiceProvider)Activator.CreateInstance(Constants.GetServiceProvider(), services);
			_singletons = provider?._singletons ?? 
				services.Where(x => x.Lifecycle == LifecycleKind.Singleton).ToDictionary(x => x.ServiceType, x => (object)null);
        }

		public object GetService(Type serviceType)
		{
			if (_singletons.ContainsKey(serviceType)) return GetOrAdd(serviceType);
			return _underlyingProvider.GetService(serviceType);
		}

		private object GetOrAdd(Type serviceType)
		{
			var service = _singletons[serviceType];
			if (service == null) _singletons[serviceType] = _underlyingProvider.GetService(serviceType);
			return _singletons[serviceType];
		}
	}
}