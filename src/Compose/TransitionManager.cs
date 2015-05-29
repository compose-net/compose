using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Compose
{
	internal class DynamicManager<T> : ITransitionManager<T> where T : class
	{
		private static List<WeakReference<DynamicManager<T>>> Managers 
			= new List<WeakReference<DynamicManager<T>>>();

		private static TypeInfo Disposable = typeof(IDisposable).GetTypeInfo();

		public T CurrentService { get; set; }
		private T SnapshotService { get; set; }

		public DynamicManager(T original)
		{
			CurrentService = original;
			Managers.Add(new WeakReference<DynamicManager<T>>(this));
		}

		protected DynamicManager(IServiceProvider provider, Func<IServiceProvider, T> factory)
			: this(factory(provider))
		{ }

		public void Change(Func<T> service)
		{
			foreach (var instance in GetActiveManagers())
				instance.Change(service());
		}

		private IEnumerable<DynamicManager<T>> GetActiveManagers()
		{
			var deadReferences = new List<WeakReference<DynamicManager<T>>>(Managers.Count);

			DynamicManager<T> manager = null;
			foreach (var reference in Managers)
				if (reference.TryGetTarget(out manager))
					yield return manager;
				else
					deadReferences.Add(reference);

			foreach (var deadReference in deadReferences)
				Managers.Remove(deadReference);
		}

		private void Change(T service)
		{
			CurrentService = service;
		}

		public void Snapshot()
		{
			foreach (var manager in GetActiveManagers())
				Snapshot(manager);
		}

		private void Snapshot(DynamicManager<T> manager)
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

		private void Restore(DynamicManager<T> manager)
		{
			if (CurrentService != null && Disposable.IsAssignableFrom(CurrentService.GetType().GetTypeInfo()))
				((IDisposable)CurrentService).Dispose();
			CurrentService = SnapshotService;
		}
	}
}
