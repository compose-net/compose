using System;

namespace Compose
{
	internal class WeakReferencingDynamicManager<Interface, OriginalService> : DynamicManager<Interface, OriginalService>
		where Interface : class where OriginalService : Interface
	{
		private readonly DynamicManagerContainer<Interface, OriginalService> _container;

		public Interface CurrentService { get; set; }
		public Interface SnapshotService { get; set; }
		private WeakReference<Interface> DynamicProxy { get; set; }

		public WeakReferencingDynamicManager(DynamicManagerContainer<Interface, OriginalService> dynamicContainer, TransitionManagerContainer transitionContainer, OriginalService original)
		{
			_container = dynamicContainer;
			transitionContainer.Add(this);
			CurrentService = original;
			SnapshotService = original;
			_container.Add(this);
		}

		public void Register(Interface dynamicProxy)
		{
			DynamicProxy = new WeakReference<Interface>(dynamicProxy);
		}

		internal bool IsActive
		{
			get
			{
				Interface dynamic = null;
				return DynamicProxy != null && DynamicProxy.TryGetTarget(out dynamic);
			}
		}

		public void Change(Func<Interface> service)
		{
			_container.Change(service);
		}
	}
}
