using System;

namespace Compose
{
    internal interface IDynamicManagerContainer<TInterface, TOriginal> where TInterface : class where TOriginal : TInterface
    {
		void Add(DynamicManager<TInterface, TOriginal> manager);
		void Change(Func<TInterface> service);
    }
}
