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
			app.Services = new ServiceCollection();
			configureServices(app.Services);
			app.Services.TryAdd(ServiceDescriptor.Singleton<DynamicEmitter, DynamicEmitter>());
			app.ApplicationServices = new WrappedServiceProvider(app.Services);
			if (app.ContainsTransitionMarkers())
				app.ApplyTransitions();
			app.ApplicationServices = new WrappedServiceProvider(app.Services);
		}

		public static void UseServices(this Application app, Func<IServiceCollection, IServiceProvider> configureServices)
		{
			app.Services = new ServiceCollection();
			app.ApplicationServices = configureServices(app.Services);
			app.Services.TryAdd(ServiceDescriptor.Singleton<DynamicEmitter, DynamicEmitter>());
			if (app.ContainsTransitionMarkers())
				app.ApplyTransitions();
			app.ApplicationServices = new WrappedServiceProvider(app.ApplicationServices);
		}

		#endregion

		#region Transitions

		public static void Transition<TService, TImplementation>(this Application app) where TImplementation : class, TService where TService : class
		{
			var transitional = app.ApplicationServices.GetRequiredService<ITransitionManager<TService>>();
			if (transitional == null) throw new InvalidOperationException($"{typeof(TService).Name} must be registered as a Transitional Service (services.AddTransitional<{typeof(TService).Name}, TImplementation>()");
			transitional.Change(() => app.ApplicationServices.GetRequiredService<TImplementation>());
        }

        internal static Type CreateProxy(this Application app, TypeInfo serviceTypeInfo)
        {
            var emitter = app.ApplicationServices.GetService<DynamicEmitter>();
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
			return app.ApplicationServices.GetRequiredService<TService>();
        }

        private static DynamicEmitter GetRegisteredDynamicEmitter(this Application app)
        {
            return app.ApplicationServices.GetRequiredService<DynamicEmitter>();
        }

		#endregion

		#region Snapshotting

		public static void Snapshot(this Application app)
		{
			app.ApplicationServices?.GetService<ITransitionManagerContainer>()?.Snapshot();
		}

		public static void Restore(this Application app)
		{
			app.ApplicationServices?.GetService<ITransitionManagerContainer>()?.Restore();
		}

		#endregion
	}
}