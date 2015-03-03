using Microsoft.Framework.DependencyInjection;
using System;

namespace Compose
{
    public class Application
    {
		public string Name { get; set; }

		internal ServiceCollection Services { get; } = new ServiceCollection();

        internal RootServiceProvider Provider { get; set; }

		public IServiceProvider HostingServices { get { return Provider; } }

		protected internal T GetRequiredService<T>() where T : class
		{
			return Provider.GetService<T>() ?? ResolveSelfBound<T>();
		}

		private T ResolveSelfBound<T>() where T : class
		{
			Services.AddTransient<T, T>();
			Provider = Provider.Extend(Services);
			return Provider.GetRequiredService<T>();
		}
	}
}