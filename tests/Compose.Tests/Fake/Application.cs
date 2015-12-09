using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Compose.Tests.Fake
{
	internal sealed class Application : Compose.Application
	{
		public IServiceCollection PreConfiguredServices { get; } = new ServiceCollection();

		protected override void PreServiceConfiguration(IServiceCollection services)
			=> services.Add(PreConfiguredServices);
	}
}
