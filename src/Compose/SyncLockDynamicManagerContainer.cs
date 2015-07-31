using System;
using System.Collections.Generic;
using System.Reflection;

namespace Compose
{
	internal sealed class SyncLockDynamicManagerContainer<TInterface, TOriginal> : IDynamicManagerContainer<TInterface, TOriginal>
		where TInterface : class where TOriginal : TInterface
	{
		private readonly List<DynamicManager<TInterface, TOriginal>> _managers
			= new List<DynamicManager<TInterface, TOriginal>>();
		private readonly object _sync = new object();

		private static TypeInfo Disposable = typeof(IDisposable).GetTypeInfo();

		public void Add(DynamicManager<TInterface, TOriginal> manager)
		{
			lock (_sync)
			{
				_managers.Add(manager);
			}
		}

		private IEnumerable<DynamicManager<TInterface, TOriginal>> GetActiveManagers()
		{
			var deadReferences = new List<DynamicManager<TInterface, TOriginal>>(_managers.Count);

			lock (_sync)
			{
				foreach (var manager in _managers)
					if (manager.IsActive)
						yield return manager;
					else
						deadReferences.Add(manager);

				foreach (var deadReference in deadReferences)
					_managers.Remove(deadReference);
			}
		}

		public void Change(Func<TInterface> service)
		{
			foreach (var instance in GetActiveManagers())
				Change(instance, service());
		}

		private void Change(DynamicManager<TInterface, TOriginal> manager, TInterface service)
		{
			if (manager.CurrentService != null && Disposable.IsAssignableFrom(manager.CurrentService.GetType().GetTypeInfo()))
				((IDisposable)manager.CurrentService).Dispose();
			manager.CurrentService = service;
		}
	}
}
