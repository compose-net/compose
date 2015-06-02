using System;
using System.Reflection;

namespace Compose
{
    internal static class DynamicManagerFactory
    {
		private static Type Exposer = typeof(DynamicManagerExposer<,>);

		internal static object ForFactory(TypeInfo dynamicManagerTypeInfo, object dynamicContainer, object transitionManager, object abstractFactory)
		{
			return Activator.CreateInstance(Exposer.MakeGenericType(dynamicManagerTypeInfo.GenericTypeArguments),
				dynamicContainer, transitionManager, abstractFactory
			);
		}

		private class DynamicManagerExposer<TInterface, TOriginal> : DynamicManager<TInterface, TOriginal>
			where TInterface : class where TOriginal : TInterface
		{
			public DynamicManagerExposer(IDynamicManagerContainer<TInterface, TOriginal> dynamicContainer, ITransitionManagerContainer transitionContainer, IAbstractFactory<TOriginal> factory)
				: base(dynamicContainer, transitionContainer, (TOriginal)factory.Create())
			{ }
        }
    }
}
