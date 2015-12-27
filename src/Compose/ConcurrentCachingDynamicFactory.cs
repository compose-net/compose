using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Compose
{
	internal sealed class ConcurrentCachingDynamicFactory : DynamicFactory
	{
		private readonly DynamicEmitter _emitter;
		private readonly ConcurrentDictionary<TypeInfo, Type> _dynamicProxyCache
			= new ConcurrentDictionary<TypeInfo, Type>();

		public ConcurrentCachingDynamicFactory(DynamicEmitter emitter)
		{
			if (emitter == null)
				throw new ArgumentNullException(nameof(emitter));
			_emitter = emitter;
		}

		public Type GetDynamicProxy(TypeInfo serviceTypeInfo)
			=> _dynamicProxyCache.GetOrAdd(serviceTypeInfo, _emitter.GetManagedDynamicProxy);
	}
}
