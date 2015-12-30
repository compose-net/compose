using System;
using System.Reflection;

namespace Compose
{
	internal static class DynamicManagerFactory
	{
		private static readonly Type Exposer = typeof(DynamicManagerExposer<,>);

		internal static object ForFactory(TypeInfo dynamicManagerTypeInfo, object dynamicContainer, object transitionManager, object abstractFactory)
			=> Activate.Type(Exposer.MakeGenericType(dynamicManagerTypeInfo.GenericTypeArguments).GetTypeInfo(), 
				dynamicContainer, transitionManager, abstractFactory
			);

		private class DynamicManagerExposer<Interface, OriginalService> : WeakReferencingDynamicManager<Interface, OriginalService>
			where Interface : class where OriginalService : Interface
		{
			public DynamicManagerExposer(DynamicManagerContainer<Interface, OriginalService> dynamicContainer, TransitionManagerContainer transitionContainer, AbstractFactory<OriginalService> factory)
				: base(dynamicContainer, transitionContainer, factory.Create())
			{ }
		}
	}
}
