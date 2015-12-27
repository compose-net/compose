using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Compose
{
	public static class DynamicExtensions
	{
		internal static Type GetProxy(this Application app, TypeInfo serviceTypeInfo)
		{
			var emitter = app.ApplicationServices.GetRequiredService<DynamicFactory>();
			return emitter.GetDynamicProxy(serviceTypeInfo);
		}
	}
}
