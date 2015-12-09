using System;
using System.Reflection;

namespace Compose
{
	internal static class KnownTypes
	{
		internal static readonly Type OpenDynamicRegister = typeof(DynamicRegister<>);
		internal static readonly Type OpenDynamicManagerContainer = typeof(DynamicManagerContainer<,>);
		internal static readonly Type OpenDynamicManagerContainerImplementation = typeof(SyncLockDynamicManagerContainer<,>);
		internal static readonly Type OpenTransitionManager = typeof(TransitionManager<>);
		internal static readonly Type OpenDynamicContainer = typeof(DynamicManagerContainer<,>);
		internal static readonly Type OpenAbstractFactory = typeof(AbstractFactory<>);
		internal static readonly Type OpenAbstractFactoryImplementation = typeof(DelegateAbstractFactory<>);
		internal static readonly Type OpenDynamicManager = typeof(DynamicManager<,>);
		internal static readonly Type OpenDynamicManagerImplementation = typeof(WeakReferencingDynamicManager<,>);

		internal static readonly TypeInfo Disposable = typeof(IDisposable).GetTypeInfo();
	}
}
