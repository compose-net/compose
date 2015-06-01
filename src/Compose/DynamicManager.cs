using System;
using System.Collections.Generic;
using System.Reflection;

namespace Compose
{
	internal sealed class DynamicManager<TInterface, TOriginal> : IDynamicRegister<TInterface>, ITransitionManager<TInterface> 
		where TInterface : class where TOriginal : TInterface
	{
		private readonly IDynamicManagerContainer<TInterface, TOriginal> _container;

		public TInterface CurrentService { get; internal set; }
		internal TInterface SnapshotService { get; set; }
		private WeakReference<TInterface> DynamicProxy { get; set; }

		public DynamicManager(IDynamicManagerContainer<TInterface, TOriginal> container, TOriginal original)
		{
			_container = container;
			CurrentService = original;
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

		public void Snapshot()
		{
			_container.Snapshot();
		}

		public void Restore()
		{
			_container.Restore();
		}
	}
}
