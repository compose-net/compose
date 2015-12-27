using System;

namespace Compose
{ 
	internal interface DynamicManagerContainer<Interface, OriginalService> where Interface : class where OriginalService : Interface
	{
		void Add(WeakReferencingDynamicManager<Interface, OriginalService> manager);
		void Change(Func<Interface> service);
	}
}
