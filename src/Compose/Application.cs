using Microsoft.Framework.DependencyInjection;
using System;

namespace Compose
{
    public class Application
    {
		internal readonly ServiceCollection services = new ServiceCollection();

		internal readonly Func<IServiceCollection, IServiceProvider> createProvider = services => new WrappedReflectionServiceProvider(services);
		protected virtual Func<IServiceCollection, IServiceProvider> CreateProvider { get { return createProvider; } }

		public string Name { get; set; }
		public IServiceProvider HostingServices { get; internal set; }

		protected T ResolveRequired<T>() where T : class
		{
			return HostingServices.GetService<T>() ?? ResolveSelfBound<T>();
		}

		private T ResolveSelfBound<T>() where T : class
		{
			services.AddTransient<T, T>();
			HostingServices = CreateProvider(services);
			return HostingServices.GetRequiredService<T>();
		}
	}
}