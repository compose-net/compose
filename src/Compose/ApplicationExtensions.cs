using Microsoft.Framework.DependencyInjection;
using System;
using System.Reflection;

namespace Compose
{
    public static class ApplicationExtensions
    {
		#region Composition

		public static void UseServices(this Application app, Action<IServiceCollection> configureServices)
		{
			app.UseServices(services =>
			{
				configureServices(services);
				if (services.ContainsTransitionMarkers())
					app.ApplyTransitions(services);
				return new WrappedServiceProvider(services);
			});
		}

		internal static void UseServices(this Application app, Func<IServiceCollection, IExtendableServiceProvider> configureServices)
		{
			app.Provider = new WrappedServiceProvider(app.Services);
            app.Provider = configureServices(app.Services);
		}

		#endregion

		#region Transitions

		public static void Transition<TService, TImplementation>(this Application app) where TImplementation : class, TService where TService : class
		{
			var transitional = app.GetRequiredService<ITransitionManager<TService>>();
			if (transitional == null) throw new InvalidOperationException($"{typeof(TService).Name} must be registered as a Transitional Service (services.AddTransitional<{typeof(TService).Name}, TImplementation>()");
			transitional.Change(() => app.GetRequiredService<TImplementation>());
        }

        internal static Type CreateProxy(this Application app, TypeInfo serviceTypeInfo)
        {
            var emitter = app.HostingServices.GetService<DynamicEmitter>();
            if (emitter == null) emitter = app.GetRegisteredDynamicEmitter();
			return emitter.GetManagedDynamicProxy(serviceTypeInfo);
        }

		internal static TService CreateProxy<TService, TInjection>(this Application app) where TService : class where TInjection : TService
			=> app.CreateProxy<TService>(typeof(TInjection).GetTypeInfo());

        internal static TService CreateProxy<TService>(this Application app, TypeInfo injectionTypeInfo) where TService : class
        {
			var serviceType = typeof(TService);
			var serviceTypeInfo = serviceType.GetTypeInfo();
			var injectionType = injectionTypeInfo.AsType();
            var proxyType = app.CreateProxy(serviceTypeInfo);
			return app.GetRequiredService<TService>();
        }

        private static DynamicEmitter GetRegisteredDynamicEmitter(this Application app)
        {
            app.Provider.Extend(new ServiceDescriptor(typeof(DynamicEmitter), typeof(DynamicEmitter), ServiceLifetime.Singleton));
            return app.GetRequiredService<DynamicEmitter>();
        }

		#endregion

		#region Snapshotting

		public static void Snapshot(this Application app)
		{
			app.HostingServices.GetService<ITransitionManagerContainer>()?.Snapshot();
		}

		public static void Restore(this Application app)
		{
			app.HostingServices.GetService<ITransitionManagerContainer>()?.Restore();
		}

		#endregion
	}
}