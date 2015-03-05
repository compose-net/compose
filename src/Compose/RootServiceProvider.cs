using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Compose
{
	internal class RootServiceProvider : BaseServiceProvider, IObserveServiceCollectionChanges
	{
		private Dictionary<Type, object> _singletons;
		private Dictionary<Type, object> _snapshot;
		private IExtendableServiceProvider _fallback;

		public RootServiceProvider(IServiceCollection services, IExtendableServiceProvider fallback)
		{
			_fallback = fallback;
			_fallback.Subscribe(this);
			_singletons = services.Where(x => x.Lifecycle == LifecycleKind.Singleton)
				.ToDictionary(x => x.ImplementationType, x => (object)null);
		}

		public void Next(ServiceDescriptor amendment)
		{
			if (amendment.Lifecycle == LifecycleKind.Singleton)
				if (!_singletons.ContainsKey(amendment.ServiceType))
					_singletons.Add(amendment.ServiceType, null);
		}

		public override object GetService(Type serviceType)
		{
			if (_singletons.ContainsKey(serviceType))
				return ResolveSingleton(serviceType);
			return _fallback.GetService(serviceType);
		}

		public override IExtendableServiceProvider Extend(ServiceDescriptor service)
		{
			if (service.Lifecycle == LifecycleKind.Singleton)
				if (!_singletons.ContainsKey(service.ImplementationType))
					_singletons.Add(service.ImplementationType, null);
			_fallback = _fallback.Extend(service);
			return this;
		}

		private object ResolveSingleton(Type serviceType)
		{
			if (_singletons[serviceType] == null)
				_singletons[serviceType] = _fallback.GetService(serviceType);
			return _singletons[serviceType];
        }

		public override void Snapshot()
		{
			_snapshot = _singletons;
			_fallback.Snapshot();
		}

		public override void Restore()
		{
			_singletons = _snapshot;
			_fallback.Restore();
		}
	}
}
