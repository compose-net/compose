using Microsoft.Framework.DependencyInjection;

namespace Compose
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddTransitional<TService, TImplementation>(this IServiceCollection services) 
			where TImplementation : ITransition<TService>, TService
		{
			return services.AddTransient<TService, TImplementation>();
		}
	}
}
