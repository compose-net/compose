using Microsoft.Framework.DependencyInjection;
using System;

namespace Compose
{
    public class ApplicationBase
    {
		internal readonly ServiceCollection services = new ServiceCollection();

		internal readonly Func<IServiceCollection, IServiceProvider> createProvider = services => new WrappedReflectionServiceProvider(services);
		protected virtual Func<IServiceCollection, IServiceProvider> CreateProvider { get { return createProvider; } }

		public string Name { get; set; }
		public IServiceProvider HostingServices { get; internal set; }

		internal T GetRequiredService<T>() where T : class
		{
			return ResolveRequired<T>();
		}

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

	public class Application : ApplicationBase
	{
		public Action Execution { get; internal set; }

		public void OnExecute(Action invoke)
		{
			Execution = invoke;
		}

		public void OnExecute<TService>(Action<TService> invoke) where TService : class
		{
			OnExecute(() => invoke(GetRequiredService<TService>()));
		}
	}

	public abstract class Application<TResult> : ApplicationBase
	{
		public Func<TResult> Execution { get; internal set; }

		public void OnExecute(Func<TResult> invoke)
		{
			Execution = invoke;
		}

		public void OnExecute<TService>(Func<TService, TResult> invoke) where TService : class
		{
			OnExecute(() => invoke(GetRequiredService<TService>()));
		}
	}

	public abstract class Application<TContext, TResult> : ApplicationBase
	{
		public Func<TContext, TResult> Execution { get; internal set; }

		public void OnExecute(Func<TContext, TResult> invoke)
		{
			Execution = invoke;
		}

		public void OnExecute<TService>(Func<TService, TContext, TResult> invoke) where TService : class
		{
			OnExecute(context => invoke(GetRequiredService<TService>(), context));
		}
	}
}