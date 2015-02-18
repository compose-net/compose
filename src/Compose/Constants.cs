using System;

namespace Compose
{
	internal static class Constants
	{
		public static Type GetServiceProvider() { return Type.GetType("Microsoft.Framework.DependencyInjection.ServiceProvider", false); }
	}
}