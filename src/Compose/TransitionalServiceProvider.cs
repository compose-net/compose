using System;
using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace Compose
{
	internal class TransitionalServiceProvider : IExtendableServiceProvider
	{
		private readonly ISingletonRepositoryServiceProvider _fallback;
		private Dictionary<Type, Type> _redirects;

		public TransitionalServiceProvider(Dictionary<Type, Type> bindingRedirects, ISingletonRepositoryServiceProvider fallback)
		{
			_redirects = bindingRedirects;
			_fallback = fallback;
		}

		public object GetService(Type serviceType)
		{
			if (_redirects.ContainsKey(serviceType))
				return _fallback.GetService(_redirects[serviceType]);
			return _fallback.GetService(serviceType);
		}

		public void Extend(ServiceDescriptor service)
		{
			if (_redirects.ContainsKey(service.ServiceType))
				ExtendTransition(service.ServiceType);
			else
				_fallback.Extend(service);
		}

		private void ExtendTransition(Type serviceType)
		{
			_fallback.AppendSingleton(serviceType);
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
			foreach (var proxy in _redirects.Select(x => _fallback.GetService(x.Value)).Cast<ITransition>())
				proxy?.Restore();
			_fallback.Restore();
		}
	}
}
