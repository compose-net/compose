using System;
using System.Collections.Generic;
using System.Reflection;

namespace Compose
{
	internal sealed class SyncLockDynamicManagerContainer<Interface, OriginalService> : DynamicManagerContainer<Interface, OriginalService>
		where Interface : class where OriginalService : Interface
	{
		private readonly List<WeakReferencingDynamicManager<Interface, OriginalService>> _managers
			= new List<WeakReferencingDynamicManager<Interface, OriginalService>>();

		public void Add(WeakReferencingDynamicManager<Interface, OriginalService> manager)
		{
			lock (_managers)
				_managers.Add(manager);
		}

		private IEnumerable<WeakReferencingDynamicManager<Interface, OriginalService>> GetActiveManagers()
		{
			var deadReferences = new List<WeakReferencingDynamicManager<Interface, OriginalService>>(_managers.Count);

			lock (_managers)
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

		public void Change(Func<Interface> service)
		{
			foreach (var instance in GetActiveManagers())
				Change(instance, service());
		}

		private void Change(WeakReferencingDynamicManager<Interface, OriginalService> manager, Interface service)
		{
			if (manager.CurrentService != null && KnownTypes.DisposableInfo.IsAssignableFrom(manager.CurrentService.GetType().GetTypeInfo()))
				((IDisposable)manager.CurrentService).Dispose();
			manager.CurrentService = service;
		}
	}
}
