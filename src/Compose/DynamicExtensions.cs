using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Compose
{
	public static class DynamicExtensions
	{
		internal static TypeInfo GetProxy(this Application app, TypeInfo serviceTypeInfo)
		{
			var emitter = app.ApplicationServices.GetRequiredService<DynamicFactory>();
			return emitter.GetDynamicProxy(serviceTypeInfo);
		}
	}
}
