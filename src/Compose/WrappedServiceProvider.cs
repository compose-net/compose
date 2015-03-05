using Microsoft.Framework.DependencyInjection;
using System;

namespace Compose
{
	internal sealed class WrappedServiceProvider : BaseServiceProvider
	{
		private readonly IServiceCollection _services;
		private IServiceProvider _fallback;
		private IServiceProvider _snapshot;

		public WrappedServiceProvider(IServiceCollection services)
		{
			_fallback = (IServiceProvider)Activator.CreateInstance(Constants.GetServiceProvider(), services);
			_services = services;
        }

		public override object GetService(Type serviceType)
		{
			return _fallback.GetService(serviceType);
		}

		public override IExtendableServiceProvider Extend(ServiceDescriptor service)
		{
			_services.Add(service);
			return new WrappedServiceProvider(_services);
		}

		public override void Snapshot()
		{
			_snapshot = _fallback;
		}

		public override void Restore()
		{
			_fallback = _snapshot;
		}
	}
}
