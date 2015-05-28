using System;
using System.Reflection;

namespace Compose
{
	internal static class Constants
	{
		public static Type GetServiceProvider()
		{
            return typeof(Microsoft.Framework.DependencyInjection.ServiceCollection).GetTypeInfo().Assembly.GetType("Microsoft.Framework.DependencyInjection.ServiceProvider", true, false);
		}
	}
}