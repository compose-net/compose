using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Compose
{
	internal sealed class ConcurrentCachingDynamicFactory : DynamicFactory
	{
		private readonly DynamicEmitter _emitter;
		private readonly ConcurrentDictionary<TypeInfo, TypeInfo> _dynamicProxyCache
			= new ConcurrentDictionary<TypeInfo, TypeInfo>();

		public ConcurrentCachingDynamicFactory(DynamicEmitter emitter)
		{
			if (emitter == null)
				throw new ArgumentNullException(nameof(emitter));
			_emitter = emitter;
		}

		public TypeInfo GetDynamicProxy(TypeInfo serviceTypeInfo)
			=> _dynamicProxyCache.GetOrAdd(serviceTypeInfo, _emitter.GetManagedDynamicProxy);
	}
}
