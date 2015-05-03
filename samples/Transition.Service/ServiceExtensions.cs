using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;

namespace Transition.Service
{
	public static class ServiceExtensions
	{
		public static IServiceCollection AddSampleService(this IServiceCollection services)
		{
			services.TryAdd(GetDefaultServices());
			return services;
		}

		private static IEnumerable<ServiceDescriptor> GetDefaultServices()
		{
			return TransitionServices.GetDefaultServices();
		}
	}
}
