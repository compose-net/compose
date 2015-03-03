using Microsoft.Framework.DependencyInjection;
using System;

namespace Compose
{
	internal static class DyanmicHelper
	{
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
			app.Provider = app.Provider.Extend(new ServiceDescriptor(typeof(DynamicEmitter), typeof(DynamicEmitter), LifecycleKind.Singleton));
			return app.GetRequiredService<DynamicEmitter>();
		}
	}
}
