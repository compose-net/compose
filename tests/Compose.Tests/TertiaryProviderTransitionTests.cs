using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Compose.Tests
{
	public class TertiaryProviderTransitionTests
	{
		[Fact]
		public static void WhenApplicationTransitionedThenResolvesTertiaryService()
		{
			var application = new Application();
			application.UseServices(services => services.AddTransient<Fake.Service, Fake.Implementation>());
			var provider = application.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.AlternativeImplementation>());
			var service = application.ApplicationServices.GetRequiredService<Fake.Service>();

			service.ServiceType.Should().Be(typeof(Fake.Implementation));
			application.Transition(provider);

			service.ServiceType.Should().Be(typeof(Fake.AlternativeImplementation));
		}
	}
}
