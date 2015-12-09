using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Compose
{
	internal sealed class ConcurrentTransitionManagerContainer : TransitionManagerContainer
	{
		private readonly ConcurrentDictionary<Type, ConcurrentBag<object>> _managers
			= new ConcurrentDictionary<Type, ConcurrentBag<object>>();

		private static readonly Type Helper = typeof(GenericHelper<>);
		private static readonly TypeInfo Disposable = typeof(IDisposable).GetTypeInfo();

		private static class GenericHelper<T>
		{
			public static void Restore(object untypedManager)
			{
				var manager = untypedManager as DynamicRegister<T>;
				if (Disposable.IsAssignableFrom(manager.CurrentService.GetType().GetTypeInfo()))
					((IDisposable)manager.CurrentService).Dispose();
				manager.CurrentService = manager.SnapshotService;
			}

			public static void Snapshot(object untypedManager)
			{
				var manager = untypedManager as DynamicRegister<T>;
				if (Disposable.IsAssignableFrom(manager.SnapshotService.GetType().GetTypeInfo()))
					((IDisposable)manager.SnapshotService).Dispose();
				manager.SnapshotService = manager.CurrentService;
			}
		}

		public void Add<T>(DynamicRegister<T> register)
		{
			var managers = _managers.GetOrAdd(typeof(T), new ConcurrentBag<object>());
			managers.Add(register);
		}

		public void Restore()
		{
			foreach (var kvp in _managers)
				Restore(kvp.Key, kvp.Value);
		}

		private void Restore(Type serviceType, ConcurrentBag<object> managers)
		{
			var restoredManagers = new ConcurrentBag<object>();
			object manager = null;
			while (!managers.IsEmpty)
				if (managers.TryTake(out manager))
					restoredManagers.Add(Restore(Helper.MakeGenericType(serviceType).GetTypeInfo(), manager));
			while (!restoredManagers.IsEmpty)
				if (restoredManagers.TryTake(out manager))
					managers.Add(manager);
		}

		private object Restore(TypeInfo helper, object manager)
		{
			helper.DeclaredMethods.Single(x => x.Name == "Restore").Invoke(null, new[] { manager });
			return manager;
		}

		public void Snapshot()
		{
			foreach (var kvp in _managers)
				Snapshot(kvp.Key, kvp.Value);
		}

		private void Snapshot(Type serviceType, ConcurrentBag<object> managers)
		{
			var snapshottedManagers = new ConcurrentBag<object>();
			object manager = null;
			while (!managers.IsEmpty)
				if (managers.TryTake(out manager))
					snapshottedManagers.Add(Snapshot(Helper.MakeGenericType(serviceType).GetTypeInfo(), manager));
			while (!snapshottedManagers.IsEmpty)
				if (snapshottedManagers.TryTake(out manager))
					managers.Add(manager);
		}

		private object Snapshot(TypeInfo helper, object manager)
		{
			helper.DeclaredMethods.Single(x => x.Name == "Snapshot").Invoke(null, new[] { manager });
			return manager;
		}
	}
}
