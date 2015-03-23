using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;

namespace Transition.Service
{
	public static class ServiceExtensions
	{
		public static IServiceCollection AddSampleService(this IServiceCollection services, IConfiguration configuration = null)
		{
			services.TryAdd(GetDefaultServices(configuration));
			return services;
		}

		private static IEnumerable<IServiceDescriptor> GetDefaultServices(IConfiguration configuration = null)
		{
			return TransitionServices.GetDefaultServices(configuration);
		}
	}
}
