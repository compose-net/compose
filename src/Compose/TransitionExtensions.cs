using Microsoft.Extensions.DependencyInjection;
using System;

namespace Compose
{
    public static class TransitionExtensions
    {
		public static void Transition<Service>(this Application app, TertiaryProvider<Service> provider)
		{
			var transitional = app.ApplicationServices.GetRequiredService<TransitionManager<Service>>();
			if (transitional == null)
				throw new InvalidOperationException($"No provider has been registered for {typeof(Service).Name}. => application.UseProvider<{typeof(Service).Name}>(services => services.AddYourProvider())");
			transitional.Change(() => provider.ApplicationServices.GetRequiredService<Service>());
		}
    }
}
