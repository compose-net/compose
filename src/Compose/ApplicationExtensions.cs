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

		public static bool Transition<TService, TImplementation>(this Application app) where TImplementation : class, TService where TService : class
		{
			var transitional = app.GetRequiredService<TService>() as ITransition<TService>;
			if (transitional == null) throw new InvalidOperationException($"{typeof(TService).Name} must be registered as a Transitional Service (services.AddTransitional<{typeof(TService).Name}, TImplementation>()");
			return transitional.Change(app.GetRequiredService<TImplementation>());
        }

        internal static TypeInfo CreateProxy(this Application app, TypeInfo serviceTypeInfo, TypeInfo injectionTypeInfo)
        {
            var emitter = app.HostingServices.GetService<DynamicEmitter>();
            if (emitter == null) emitter = app.GetRegisteredDynamicEmitter();

            return emitter.GetManagedDynamicProxy(serviceTypeInfo, injectionTypeInfo);
        }

		internal static TService CreateProxy<TService, TInjection>(this Application app) where TService : class where TInjection : TService
			=> app.CreateProxy<TService>(typeof(TInjection).GetTypeInfo());

        internal static TService CreateProxy<TService>(this Application app, TypeInfo injectionTypeInfo) where TService : class
        {
			var serviceType = typeof(TService).GetTypeInfo();
            var proxyType = app.CreateProxy(serviceType, injectionTypeInfo);
			if (!proxyType.IsGenericType) return (TService)Activator.CreateInstance(proxyType.AsType(), app.GetRequiredService<TService>());

			var constructedProxy = proxyType.MakeGenericType(serviceType.GenericTypeArguments);
			var proxy = (ITransition<TService>)Activator.CreateInstance(constructedProxy, app.GetRequiredService<TService>());
			return (TService)proxy;
        }

        private static DynamicEmitter GetRegisteredDynamicEmitter(this Application app)
        {
            app.Provider.Extend(new ServiceDescriptor(typeof(DynamicEmitter), typeof(DynamicEmitter), ServiceLifetime.Singleton));
            return app.GetRequiredService<DynamicEmitter>();
        }

		#endregion

		#region Snapshotting

		public static void Snapshot(this Application app) { app.CreateSnapshot(); }

		public static void Restore(this Application app) { app.RestoreSnapshot(); }

		#endregion
	}
}