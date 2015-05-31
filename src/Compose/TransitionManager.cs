using System;
using System.Collections.Generic;
using System.Reflection;

namespace Compose
{
	internal class DynamicManager<TInterface, TOriginal> : IDynamicRegister<TInterface>, ITransitionManager<TInterface> 
		where TInterface : class where TOriginal : TInterface
	{
		private static List<DynamicManager<TInterface, TOriginal>> Managers 
			= new List<DynamicManager<TInterface, TOriginal>>();

		private static TypeInfo Disposable = typeof(IDisposable).GetTypeInfo();

		public TInterface CurrentService { get; set; }
		private TInterface SnapshotService { get; set; }
		private WeakReference<TInterface> DynamicProxy { get; set; }

		public DynamicManager(TOriginal original)
		{
			CurrentService = original;
			Managers.Add(this);
		}

		protected DynamicManager(IServiceProvider provider, Func<IServiceProvider, TOriginal> factory)
			: this(factory(provider))
		{ }

		public void Register(TInterface dynamicProxy)
		{
			DynamicProxy = new WeakReference<TInterface>(dynamicProxy);
		}

		private bool IsActive
		{
			get
			{
				TInterface dynamic = null;
				return DynamicProxy != null && DynamicProxy.TryGetTarget(out dynamic);
			}
		}

		public void Change(Func<TInterface> service)
		{
			foreach (var instance in GetActiveManagers())
				instance.Change(service());
		}

		private static IEnumerable<DynamicManager<TInterface, TOriginal>> GetActiveManagers()
		{
			var deadReferences = new List<DynamicManager<TInterface, TOriginal>>(Managers.Count);

			foreach (var manager in Managers)
				if (manager.IsActive)
					yield return manager;
				else
					deadReferences.Add(manager);

			foreach (var deadReference in deadReferences)
				Managers.Remove(deadReference);
		}

		private void Change(TInterface service)
		{
			CurrentService = service;
		}

		public void Snapshot()
		{
			foreach (var manager in GetActiveManagers())
				Snapshot(manager);
		}

		private void Snapshot(DynamicManager<TInterface, TOriginal> manager)
		{
			if (SnapshotService != null && Disposable.IsAssignableFrom(SnapshotService.GetType().GetTypeInfo()))
				((IDisposable)SnapshotService).Dispose();
			SnapshotService = CurrentService;
		}

		public void Restore()
		{
			foreach (var manager in GetActiveManagers())
				Restore(manager);
		}

		private void Restore(DynamicManager<TInterface, TOriginal> manager)
		{
			if (CurrentService != null && Disposable.IsAssignableFrom(CurrentService.GetType().GetTypeInfo()))
				((IDisposable)CurrentService).Dispose();
			CurrentService = SnapshotService;
		}
	}
}
