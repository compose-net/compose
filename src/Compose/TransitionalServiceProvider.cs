using System;
using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace Compose
{
	internal class TransitionalServiceProvider : BaseServiceProvider
	{
		private Dictionary<Type, Type> _redirects;
		private IExtendableServiceProvider _fallback;

		public TransitionalServiceProvider(Dictionary<Type, Type> bindingRedirects, IExtendableServiceProvider fallback)
		{
			_redirects = bindingRedirects;
			_fallback = fallback;
		}

		public override object GetService(Type serviceType)
		{
			if (_redirects.ContainsKey(serviceType))
				return _fallback.GetService(_redirects[serviceType]);
			return _fallback.GetService(serviceType);
		}

		public override IExtendableServiceProvider Extend(ServiceDescriptor service)
		{
			if (_redirects.ContainsKey(service.ServiceType))
				_fallback = _fallback.Extend(GetBubbledAmendment(service.ServiceType));
			else
				_fallback = _fallback.Extend(service);
			return this;
		}

		private ServiceDescriptor GetBubbledAmendment(Type serviceType)
		{
            PublishChange(new ServiceDescriptor(serviceType, _redirects[serviceType], LifecycleKind.Singleton));
			return new ServiceDescriptor(_redirects[serviceType], _redirects[serviceType], LifecycleKind.Singleton);
		}

		public override void Snapshot()
		{
			foreach (var proxy in _redirects.Select(x => _fallback.GetService(x.Value)).Where(x => x != null).Cast<ITransition>())
				proxy.Snapshot();
			_fallback.Snapshot();
		}

		public override void Restore()
		{
			foreach (var proxy in _redirects.Select(x => _fallback.GetService(x.Value)).Where(x => x != null).Cast<ITransition>())
				proxy.Restore();
			_fallback.Restore();
		}
	}
}
