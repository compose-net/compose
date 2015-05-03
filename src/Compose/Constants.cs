using System;

namespace Compose
{
	internal static class Constants
	{
		public static Type GetServiceProvider()
		{
            return typeof(Microsoft.Framework.DependencyInjection.ServiceCollection).Assembly.GetType("Microsoft.Framework.DependencyInjection.ServiceProvider", true);
		}
	}
}