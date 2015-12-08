using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Compose
{
	public static class DynamicExtensions
	{
		internal static Type CreateProxy(this Application app, TypeInfo serviceTypeInfo)
		{
			var emitter = app.ApplicationServices.GetService<DynamicEmitter>();
			if (emitter == null) emitter = app.GetRegisteredDynamicEmitter();
			return emitter.GetManagedDynamicProxy(serviceTypeInfo);
		}

		private static DynamicEmitter GetRegisteredDynamicEmitter(this Application app)
			=> app.ApplicationServices.GetRequiredService<DynamicEmitter>();
	}
}
