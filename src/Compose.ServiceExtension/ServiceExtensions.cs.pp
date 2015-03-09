using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;

namespace $rootnamespace$
{
	public static class ServiceExtensions
	{
		public static IServiceCollection AddYourService(this IServiceCollection services, IConfiguration configuration = null)
		{
			services.TryAdd(GetDefaultServices(configuration));
			return services;
		}

		private static IEnumerable<IServiceDescriptor> GetDefaultServices(IConfiguration configuration = null)
		{
			var describe = new ServiceDescriber(configuration);

			// TODO: yield your service bindings

			yield return describe.Transient<YourService, YourService>();
		}

		internal class YourService { }
	}
}
