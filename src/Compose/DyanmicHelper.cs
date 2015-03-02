using Microsoft.Framework.DependencyInjection;
using System;

namespace Compose
{
	internal static class DyanmicHelper
	{
		internal static TService CreateProxy<TService>(this Application app) where TService : class
		{
			var emitter = app.HostingServices.GetService<DynamicEmitter>();
			if (emitter == null) emitter = app.GetRegisteredDynamicEmitter();

			return emitter.GetDirectTransitionImplementation(app.GetRequiredService<TService>());
		}

		private static DynamicEmitter GetRegisteredDynamicEmitter(this Application app)
		{
			app.Provider = app.Provider.Extend(app.Services.AddSingleton<DynamicEmitter>());
			return app.GetRequiredService<DynamicEmitter>();
		}
	}
}
