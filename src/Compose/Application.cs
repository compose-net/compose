using Microsoft.Framework.DependencyInjection;
using System;

namespace Compose
{
    public class Application
    {
		public string Name { get; set; }

		public IServiceProvider HostingServices { get { return Provider; } }

		protected internal T GetRequiredService<T>() where T : class
		{
			return Provider.GetService<T>() ?? ResolveSelfBound<T>();
		}

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