using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Compose.Tests.Fake
{
	internal sealed class Application : Compose.Application
	{
        public IServiceCollection PreConfiguredServices { get; } = new ServiceCollection();
		public IReadOnlyCollection<ServiceDescriptor> PostConfiguredServices { get; set; }
        public Action<IReadOnlyCollection<ServiceDescriptor>>  PostConfigurationCallback { private get; set; }
		public bool PostConfigurationCalled { get; private set; }

		protected override void PreServiceConfiguration(IServiceCollection services)
			=> services.Add(PreConfiguredServices);

		protected override void PostServiceConfiguration(IReadOnlyCollection<ServiceDescriptor> services)
		{
            PostConfiguredServices = services;
			PostConfigurationCallback?.Invoke(services);
			PostConfigurationCalled = true;
		}
	}
}
