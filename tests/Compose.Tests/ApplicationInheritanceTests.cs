using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TestAttributes;

namespace Compose.Tests
{
	public class ApplicationInheritanceTests
	{
		public class GivenUsingBuiltInProvider
		{
			[Unit]
			public static void WhenServicesAddedToPreServiceConfigurationThenServicesCanBeResolved()
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
			public static void WhenServicesAddedToPreServiceConfigurationThenServicesArePresentInPostConfiguration()
			{
				var app = new Fake.Application();
				app.PreConfiguredServices.AddTransient<Fake.Service, Fake.Implementation>();
				app.PostConfigurationCallback = services => services.Should().Contain(app.PreConfiguredServices);

				app.PostConfigurationCalled.Should().BeFalse();
				app.ApplicationServices.Should().NotBeNull();

				app.PostConfigurationCalled.Should().BeTrue();
			}

			[Unit]
			public static void WhenServicesAddedThenServicesArePresentInPostConfiguration()
			{
				var app = new Fake.Application();
				var serviceDescriptor = ServiceDescriptor.Transient<Fake.Service, Fake.Implementation>();
				app.PostConfigurationCallback = services => services.Should().Contain(serviceDescriptor);
				app.UseServices(services => services.Add(serviceDescriptor));

				app.PostConfigurationCalled.Should().BeFalse();
				app.ApplicationServices.Should().NotBeNull();

				app.PostConfigurationCalled.Should().BeTrue();
			}
		}

		public class GivenUsingCustomProvider
		{
			[Unit]
			public static void WhenServicesAddedToPreServiceConfigurationThenServicesCanBeResolved()
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
			public static void WhenServicesAddedToPreServiceConfigurationThenServicesArePresentInPostConfiguration()
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
			public static void WhenServicesAddedThenServicesArePresentInPostConfiguration()
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
