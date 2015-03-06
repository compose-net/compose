using Microsoft.Framework.DependencyInjection;
using System;

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
					return new TransitionalServiceProvider(app.GetTransitionalRedirects(services), new WrappedServiceProvider(services));
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

		public static bool Transition<TService, TImplementation>(this Application app) where TImplementation : class, TService
		{
			var transitional = app.HostingServices.GetService<TService>() as ITransition<TService>;
			if (transitional == null) throw new InvalidOperationException($"{typeof(TService).Name} must be registered as a Transitional Service (services.AddTransitional<{typeof(TService).Name}, TImplementation>()");
			return transitional.Change(app.GetRequiredService<TImplementation>());
        }

        internal static Type CreateProxy(this Application app, Type service)
        {
            var emitter = app.HostingServices.GetService<DynamicEmitter>();
            if (emitter == null) emitter = app.GetRegisteredDynamicEmitter();

            return emitter.GetDirectTransitionImplementation(service);
        }

        internal static TService CreateProxy<TService>(this Application app) where TService : class
        {
            return (TService)Activator.CreateInstance(app.CreateProxy(typeof(TService)), app.GetRequiredService<TService>());
        }

        private static DynamicEmitter GetRegisteredDynamicEmitter(this Application app)
        {
            app.Provider.Extend(new ServiceDescriptor(typeof(DynamicEmitter), typeof(DynamicEmitter), LifecycleKind.Singleton));
            return app.GetRequiredService<DynamicEmitter>();
        }

		#endregion

		#region Snapshotting

		public static void Snapshot(this Application app) { app.CreateSnapshot(); }

		public static void Restore(this Application app) { app.RestoreSnapshot(); }

		#endregion
	}
}