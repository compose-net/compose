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
			_underlyingProvider = (IServiceProvider)Activator.CreateInstance(Constants.GetServiceProvider());
        }

		public object GetService(Type serviceType)
		{
			throw new NotImplementedException();
		}
	}
}