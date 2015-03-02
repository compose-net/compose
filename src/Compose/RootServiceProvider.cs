using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Compose
{
	internal class RootServiceProvider : IServiceProvider
	{
		private readonly IServiceProvider _provider;
		private Dictionary<Type, object> _singletons;

		public RootServiceProvider(IServiceProvider provider)
		{
			_provider = provider;
		}

		public RootServiceProvider(IServiceProvider provider, RootServiceProvider root) : this(provider)
		{
			_singletons = root._singletons;
		}

		internal virtual RootServiceProvider Extend(IEnumerable<IServiceDescriptor> services)
		{
			return new RootServiceProvider(new WrappedServiceProvider(services), this);
		}

		public virtual object GetService(Type serviceType)
		{
			return resolveSingleton(serviceType) ?? _provider.GetService(serviceType);
		}

		private object resolveSingleton(Type serviceType)
		{
			if (_singletons == null) _singletons = new Dictionary<Type, object>();
			if (!_singletons.ContainsKey(serviceType)) return null;
			if (_singletons[serviceType] == null) _singletons[serviceType] = _provider.GetService(serviceType);
			return _singletons[serviceType];
		}
	}
}
