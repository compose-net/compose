using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Compose
{
	internal sealed class WrappedServiceProvider : ISingletonRepositoryServiceProvider
	{
		private readonly IServiceCollection _services;
		private SingletonRegister _singletons;
		private IServiceProvider _fallback;
		private IServiceProvider _snapshot;

		public WrappedServiceProvider(IServiceCollection services)
		{
			_fallback = services.BuildServiceProvider();
			_singletons = services.BuildSingletonRegister();
			_services = services;
        }

		public object GetService(Type serviceType)
		{
			if (_singletons.CanResolveSingleton(serviceType))
				return _singletons.Resolve(ref _fallback, serviceType);
			// exception logic - neccessary evil due to bug in beta 4 MS Provider
			try
			{
				return _fallback.GetService(serviceType);
			}
			catch (ArgumentNullException anex)
			{
				if (anex.Message == "Object cannot be null.\r\nParameter name: source") return null;
				throw;
			}
		}

		public void Extend(ServiceDescriptor service)
		{
			_services.Add(service);
			_fallback = _services.BuildServiceProvider();
		}

		public void AppendSingleton(Type serviceType)
		{
			if (_singletons.CanResolveSingleton(serviceType)) return;
			_services.AddSingleton(serviceType);
			_fallback = _services.BuildServiceProvider();
		}

		public void Snapshot()
		{
			_snapshot = _services.BuildServiceProvider();
		}

		public void Restore()
		{
			_fallback = _snapshot ?? _fallback;
		}
	}
}
