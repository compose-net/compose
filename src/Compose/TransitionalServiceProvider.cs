using System;
using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace Compose
{
	internal class TransitionalServiceProvider : IExtendableServiceProvider
	{
		private readonly ISingletonRepositoryServiceProvider _fallback;
		private readonly Dictionary<Type, object> _transitions = new Dictionary<Type, object>();
		private Dictionary<Type, Type> _redirects;

		public TransitionalServiceProvider(Dictionary<Type, Type> bindingRedirects, ISingletonRepositoryServiceProvider fallback)
		{
			_redirects = bindingRedirects;
			_fallback = fallback;
		}

		public object GetService(Type serviceType)
		{
			if (_redirects.ContainsKey(serviceType))
				return ResolveTransition(serviceType);
			return _fallback.GetService(serviceType);
		}

		public void Extend(ServiceDescriptor service)
		{
			if (_redirects.ContainsKey(service.ServiceType))
				ExtendTransition(service.ServiceType);
			else
				_fallback.Extend(service);
		}

		private object ResolveTransition(Type serviceType)
		{
			if (_transitions.ContainsKey(serviceType))
				return _transitions[serviceType]; // fallback captures these as singletons anyway
			var transition = _fallback.GetService(_redirects[serviceType]);
			if (transition == null) return transition;
			((ITransition)transition).Snapshot();
			return transition;
        }

		private void ExtendTransition(Type serviceType)
		{
			_fallback.AppendSingleton(_redirects[serviceType]);
			_fallback.Extend(new ServiceDescriptor(_redirects[serviceType], _redirects[serviceType], LifecycleKind.Singleton));
		}

		public void Snapshot()
		{
			foreach (var proxy in _redirects.Select(x => _fallback.GetService(x.Value)).Cast<ITransition>())
				proxy?.Snapshot();
			_fallback.Snapshot();
		}

		public void Restore()
		{
			_fallback.Restore();
			foreach (var proxy in _redirects.Select(x => _fallback.GetService(x.Value)).Cast<ITransition>())
				proxy?.Restore();
		}
	}
}
