using Microsoft.Framework.DependencyInjection;

namespace Compose
{
	public class ApplicationBase
	{
		internal ServiceCollection Services { get; } = new ServiceCollection();

		internal IExtendableServiceProvider Provider { get; set; }

		internal T ResolveSelfBound<T>()
		{
			var serviceType = typeof(T);
			Provider = Provider.Extend(new ServiceDescriptor(serviceType, serviceType, LifecycleKind.Transient));
			return Provider.GetRequiredService<T>();
		}
	}
}
