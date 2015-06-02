using Microsoft.Framework.DependencyInjection;

namespace Compose.Tests
{
    internal static class ApplicationExtensions
    {
		internal static bool CanResolveMultipleInstances<T>(this Application app)
		{
			var instance1 = app.ApplicationServices.GetRequiredService<T>();
			var instance2 = app.ApplicationServices.GetRequiredService<T>();
			return !instance1.Equals(instance2);
		}
	}
}
