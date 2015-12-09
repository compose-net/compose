using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Compose.Tests
{
	public class ApplicationInheritanceTests
	{
		[Fact]
		public static void GivenBuiltInProviderWhenServicesAddedToPreServiceConfigurationThenServicesCanBeResolved()
		{
			var app = new Fake.Application();
			app.PreConfiguredServices.AddTransient<Fake.Service, Fake.Implementation>();
			app.UseServices(services =>
			{
				services.Should().Contain(app.PreConfiguredServices);
			});
			app.ApplicationServices.Should().NotBeNull();
		}

		[Fact]
		public static void GivenCustomProviderWhenServicesAddedToPreServiceConfigurationThenServicesCanBeResolved()
		{
			var provider = new Mock<IServiceProvider>().Object;
			var app = new Fake.Application();
			app.PreConfiguredServices.AddTransient<Fake.Service, Fake.Implementation>();
			app.UseServices(services =>
			{
				services.Should().Contain(app.PreConfiguredServices);
				return provider;
			});
			app.ApplicationServices.Should().NotBeNull();
		}
	}
}
