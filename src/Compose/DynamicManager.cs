using System;
using System.Collections.Generic;
using System.Reflection;

namespace Compose
{
	internal class DynamicManager<TInterface, TOriginal> : IDynamicManager<TInterface, TOriginal>
		where TInterface : class where TOriginal : TInterface
	{
		private readonly IDynamicManagerContainer<TInterface, TOriginal> _container;

		public TInterface CurrentService { get; set; }
		public TInterface SnapshotService { get; set; }
		private WeakReference<TInterface> DynamicProxy { get; set; }

		public DynamicManager(IDynamicManagerContainer<TInterface, TOriginal> dynamicContainer, ITransitionManagerContainer transitionContainer, TOriginal original)
		{
			_container = dynamicContainer;
			transitionContainer.Add(this);
			CurrentService = original;
			SnapshotService = original;
			_container.Add(this);
		}

		public void Register(TInterface dynamicProxy)
		{
			DynamicProxy = new WeakReference<TInterface>(dynamicProxy);
		}

		internal bool IsActive
		{
			get
			{
				TInterface dynamic = null;
				return DynamicProxy != null && DynamicProxy.TryGetTarget(out dynamic);
			}
		}

		public void Change(Func<TInterface> service)
		{
			_container.Change(service);
		}
	}
}
