using Microsoft.Framework.DependencyInjection;
using System;

namespace Compose
{
	public static class ApplicationExtensions
	{
		public static bool Transition<TService, TImplementation>(this Application app)
			where TImplementation : TService
		{
			var transitional = app.HostingServices.GetService<ITransition<TService>>();
			if (transitional == null) throw new InvalidOperationException($"{typeof(TService).Name} must be registered as a Transitional Service (services.AddTransitional<{typeof(TService).Name}, TImplementation>()");
			return transitional.Change<TImplementation>();
		}
	}
}
