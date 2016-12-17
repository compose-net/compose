using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using TestAttributes;
using Xunit;

namespace Compose.Tests
{
	public class ApplicationTests
	{
        class Service : Fake.Service
        { }

		[Unit]
		public void WhenUsingInternalServiceProviderThenPreConfigureServicesReceivesEmptyServiceCollection()
		{
			var app = new Fake.Application();
			app.UseServices(services => { });

			Assert.NotNull(app.PreConfiguredServices);
			Assert.Equal(0, app.PreConfiguredServices.Count);
		}

		[Unit]
		public void WhenUsingInternalServiceProviderThenPostConfigureServicesReceivesConfiguredServices()
		{
			var app = new Fake.Application();
			app.UseServices(services => services.AddTransient<Service>());

			Assert.NotNull(app.PostConfiguredServices);
			Assert.Contains(app.PostConfiguredServices, service => service.ServiceType == typeof(Service));
		}

		[Unit]
		public void WhenUsingInternalServiceProviderThenPreConfiguredServicesArePresentInApplicationServices()
		{
			var app = new Fake.Application
			{
				ServicesToAppendPreConfiguration = new List<ServiceDescriptor>()
				{
					ServiceDescriptor.Transient<Fake.Service, Service>()
				}
			};

			app.UseServices(services => { });


			Assert.NotNull(app.PostConfiguredServices);
			Assert.Contains(app.PostConfiguredServices, service => service.ServiceType == typeof(Fake.Service));
		}

		[Unit]
		public void WhenUsingCustomServiceProviderThenPreConfigureServicesReceivesEmptyServiceCollection()
		{
			var app = new Fake.Application();
			app.UseServices(services => default(IServiceProvider));

			Assert.NotNull(app.PreConfiguredServices);
			Assert.Equal(0, app.PreConfiguredServices.Count);
		}

		[Unit]
		public void WhenUsingCustomServiceProviderThenPostConfigureServicesReceivesConfiguredServices()
		{
			var app = new Fake.Application();
			app.UseServices(services =>
			{
				services.AddTransient<Fake.Service>();
				return default(IServiceProvider);
			});

			Assert.NotNull(app.PostConfiguredServices);
			Assert.Contains(app.PostConfiguredServices, service => service.ServiceType == typeof(Fake.Service));
		}

		[Unit]
		public void WhenUsingCustomServiceProviderThenPreConfiguredServicesArePresentInApplicationServices()
		{
			var app = new Fake.Application
			{
				ServicesToAppendPreConfiguration = new List<ServiceDescriptor>()
				{
					ServiceDescriptor.Transient<Fake.Service, Service>()
				}
			};

			app.UseServices(services => default(IServiceProvider));


			Assert.NotNull(app.PostConfiguredServices);
			Assert.Contains(app.PostConfiguredServices, service => service.ServiceType == typeof(Fake.Service));
		}
	}
}
