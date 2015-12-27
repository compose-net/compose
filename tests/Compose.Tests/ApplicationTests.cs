using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using TestAttributes;

namespace Compose.Tests
{
	public class ApplicationTests
	{
		[Unit]
		public void WhenUsingInternalServiceProviderThenPreConfigureServicesReceivesEmptyServiceCollection()
		{
			var app = new Fake.Application();
			app.UseServices(services => { });

			app.PreConfiguredServices.Should().NotBeNull();
			app.PreConfiguredServices.Count.Should().Be(0);
		}

		[Unit]
		public void WhenUsingInternalServiceProviderThenPostConfigureServicesReceivesConfiguredServices()
		{
			var app = new Fake.Application();
			app.UseServices(services => services.AddTransient<Fake.Service, Fake.Implementation>());
			var serviceProvider = app.ApplicationServices;

			app.PostConfiguredServices.Should().NotBeNull();
			app.PostConfiguredServices.Should().Contain(service => service.ServiceType == typeof(Fake.Service));
		}

		[Unit]
		public void WhenUsingInternalServiceProviderThenPreConfiguredServicesArePresentInApplicationServices()
		{
			var app = new Fake.Application();

			app.UseServices(services =>
			{
				services.AddTransient<Fake.Service, Fake.Implementation>();
			});

			var serviceProvider = app.ApplicationServices;

			app.PostConfiguredServices.Should().NotBeNull();
			app.PostConfiguredServices.Should().Contain(service => service.ServiceType == typeof(Fake.Service));
		}

		[Unit]
		public void WhenUsingCustomServiceProviderThenPreConfigureServicesReceivesEmptyServiceCollection()
		{
			var app = new Fake.Application();
			app.UseServices(services => default(IServiceProvider));

			app.PreConfiguredServices.Should().NotBeNull();
			app.PreConfiguredServices.Count.Should().Be(0);
		}

		[Unit]
		public void WhenUsingCustomServiceProviderThenPostConfigureServicesReceivesConfiguredServices()
		{
			var app = new Fake.Application();
			app.UseServices(services =>
			{
				services.AddTransient<Fake.Service, Fake.Implementation>();
				return default(IServiceProvider);
			});
			var serviceProvider = app.ApplicationServices;

			app.PostConfiguredServices.Should().NotBeNull();
			app.PostConfiguredServices.Should().Contain(service => service.ServiceType == typeof(Fake.Service));
		}
	}
}
