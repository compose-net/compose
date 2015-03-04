using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;

namespace Transition.Service
{
	public static class ServiceExtensions
	{
		public static IServiceCollection AddSampleServices(this IServiceCollection services, IConfiguration configuration = null)
		{
			services.TryAdd(TransitionServices.GetDefaultServices(configuration));
			return services;
		}
	}
}
