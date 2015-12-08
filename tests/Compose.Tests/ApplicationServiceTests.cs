using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Compose.Tests
{
	public class ApplicationServiceTests
	{
		[Fact]
		public static void GivenApplicationHasNoServicesWhenGettingApplicationServicesThenReturnsBaseProvider()
		{
			new Application().ApplicationServices
				.Should().NotBeNull();
		}

		[Fact]
		public static void WhenConfiguringApplicationServicesToUseInternalProviderThenReturnsProviderContainingService()
		{
			var application = new Application();
			application.UseServices(services => services.AddTransient<Fake.Implementation>());
			application.ApplicationServices.GetService<Fake.Implementation>()
				.Should().NotBeNull();
		}

		[Fact]
		public static void WhenConfiguringApplicationServicesToUseCustomProviderThenApplicationServicesMatchesCustomProvider()
		{
			var provider = new Mock<IServiceProvider>().Object;
			var application = new Application();
			application.UseServices(_ => provider);
			application.ApplicationServices
				.Should().Be(provider);
		}
	}
}
