using System;

namespace Compose
{
    internal static class DynamicManagerFactory
    {
		private static Type DynamicManagerType = typeof(DynamicManager<,>);

		internal static Type ForType(Type serviceType, Type implementationType)
		{
			return DynamicManagerType.MakeGenericType(serviceType, implementationType);
		}

		internal static object ForInstance(Type serviceType, object implementationInstance)
		{
			return Activator.CreateInstance(DynamicManagerType.MakeGenericType(serviceType, implementationInstance.GetType()), implementationInstance);
		}

		internal static Func<IServiceProvider, object> ForFactory(Func<IServiceProvider, object> implementationFactory, Type dynamicManagerType, Type dynamicProxyType)
		{
			return new DynamicFactory(implementationFactory, dynamicManagerType, dynamicProxyType).Create;
		}
    }
}
