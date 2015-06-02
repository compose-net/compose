using Microsoft.Framework.DependencyInjection;
using System;

namespace Compose
{
	internal sealed class WrappedServiceProvider : IServiceProvider
	{
		private IServiceProvider _fallback;

		public WrappedServiceProvider(IServiceCollection services)
		{
			_fallback = services.BuildServiceProvider();
        }

		public WrappedServiceProvider(IServiceProvider fallback)
		{
			_fallback = fallback;
		}

		public object GetService(Type serviceType)
		{
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
	}
}
