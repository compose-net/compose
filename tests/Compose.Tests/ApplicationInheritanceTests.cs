using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TestAttributes;

namespace Compose.Tests
{
	public class ApplicationInheritanceTests
	{
		[Unit]
		public static void GivenUsingBuiltInProviderWhenServicesAddedToPreServiceConfigurationThenServicesCanBeResolved()
		{
			var app = new Fake.Application();
			app.PreConfiguredServices.AddTransient<Fake.Service, Fake.Implementation>();
			var useServicesCalled = false;
			app.UseServices(services =>
			{
				services.Should().Contain(app.PreConfiguredServices);
				useServicesCalled = true;
			});

			app.ApplicationServices.Should().NotBeNull();

			useServicesCalled.Should().BeTrue();
		}

		[Unit]
		public static void GivenUsingBuiltInProviderWhenServicesAddedToPreServiceConfigurationThenServicesArePresentInPostConfiguration()
		{
			var app = new Fake.Application();
			app.PreConfiguredServices.AddTransient<Fake.Service, Fake.Implementation>();
			app.PostConfigurationCallback = services => services.Should().Contain(app.PreConfiguredServices);

			app.PostConfigurationCalled.Should().BeFalse();
			app.ApplicationServices.Should().NotBeNull();

			app.PostConfigurationCalled.Should().BeTrue();
		}

		[Unit]
		public static void GivenUsingBuiltInProviderWhenServicesAddedThenServicesArePresentInPostConfiguration()
		{
			var app = new Fake.Application();
			var serviceDescriptor = ServiceDescriptor.Transient<Fake.Service, Fake.Implementation>();
			app.PostConfigurationCallback = services => services.Should().Contain(serviceDescriptor);
			app.UseServices(services => services.Add(serviceDescriptor));

			app.PostConfigurationCalled.Should().BeFalse();
			app.ApplicationServices.Should().NotBeNull();

			app.PostConfigurationCalled.Should().BeTrue();
		}

		[Unit]
		public static void GivenUsingCustomProviderWhenServicesAddedToPreServiceConfigurationThenServicesCanBeResolved()
		{
			var provider = new Mock<IServiceProvider>().Object;
			var app = new Fake.Application();
			app.PreConfiguredServices.AddTransient<Fake.Service, Fake.Implementation>();
			var useServicesCalled = false;
			app.UseServices(services =>
			{
				services.Should().Contain(app.PreConfiguredServices);
				useServicesCalled = true;
				return provider;
			});

			app.ApplicationServices.Should().NotBeNull();

			useServicesCalled.Should().BeTrue();
		}

		[Unit]
		public static void GivenUsingCustomProviderWhenServicesAddedToPreServiceConfigurationThenServicesArePresentInPostConfiguration()
		{
			var provider = new Mock<IServiceProvider>().Object;
			var app = new Fake.Application();
			app.PreConfiguredServices.AddTransient<Fake.Service, Fake.Implementation>();
			app.PostConfigurationCallback = services => services.Should().Contain(app.PreConfiguredServices);
			app.UseServices(_ => provider);

			app.PostConfigurationCalled.Should().BeFalse();
			app.ApplicationServices.Should().NotBeNull();

			app.PostConfigurationCalled.Should().BeTrue();
		}

		[Unit]
		public static void GivenUsingCustomProviderWhenServicesAddedThenServicesArePresentInPostConfiguration()
		{
			var provider = new Mock<IServiceProvider>().Object;
			var app = new Fake.Application();
			var serviceDescriptor = ServiceDescriptor.Transient<Fake.Service, Fake.Implementation>();
			app.PostConfigurationCallback = services => services.Should().Contain(serviceDescriptor);
			app.UseServices(services =>
			{
				services.Add(serviceDescriptor);
				return provider;
			});

			app.PostConfigurationCalled.Should().BeFalse();
			app.ApplicationServices.Should().NotBeNull();

			app.PostConfigurationCalled.Should().BeTrue();
		}
	}
}
}