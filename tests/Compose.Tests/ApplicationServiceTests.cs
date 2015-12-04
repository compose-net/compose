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
		public void GivenApplicationHasNoServicesWhenGettingApplicationServicesThenReturnsBaseProvider()
		{
			new Application().ApplicationServices
				.Should().NotBeNull();
		}

		[Fact]
		public void WhenConfiguringApplicationServicesToUseInternalProviderThenReturnsProviderContainingService()
		{
			var application = new Application();
			application.UseServices(services => services.AddTransient<Fake.Implementation>());
			application.ApplicationServices.GetService<Fake.Implementation>()
				.Should().NotBeNull();
		}

		[Fact]
		public void WhenConfiguringApplicationServicesToUseCustomProviderThenApplicationServicesMatchesCustomProvider()
		{
			var provider = new Mock<IServiceProvider>().Object;
			var application = new Application();
			application.UseServices(_ => provider);
			application.ApplicationServices
				.Should().Be(provider);
		}
    }
}
