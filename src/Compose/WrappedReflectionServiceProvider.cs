using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Compose
{
	internal sealed class WrappedReflectionServiceProvider : IServiceProvider
	{
		private readonly IServiceProvider _underlyingProvider;

		public WrappedReflectionServiceProvider(IEnumerable<IServiceDescriptor> services)
		{
			_underlyingProvider = (IServiceProvider)Activator.CreateInstance(Constants.GetServiceProvider(), services);
        }

		public object GetService(Type serviceType)
		{
			return _underlyingProvider.GetService(serviceType);
		}
	}
}