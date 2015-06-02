using System;

namespace Compose
{
    internal sealed class DynamicFactory
    {
		private readonly Func<IServiceProvider, object> _original;
		private readonly Type _dynamicManagerType;
		private readonly Type _dynamicProxyType;

		public DynamicFactory(Func<IServiceProvider, object> original, Type dynamicManagerType, Type dynamicProxyType)
		{
			_original = original;
			_dynamicManagerType = dynamicManagerType;
			_dynamicProxyType = dynamicProxyType;
		}

		public object Create(IServiceProvider provider)
		{
			return Activator.CreateInstance(_dynamicProxyType, new[] { Activator.CreateInstance(_dynamicManagerType, new[] { _original(provider) }) });
		}
    }
}
