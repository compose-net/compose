using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Compose
{
	public class Application : ServiceProvider
	{
		protected internal virtual void PreServiceConfiguration(IServiceCollection services) { }

		protected internal virtual void PostServiceConfiguration(IReadOnlyCollection<ServiceDescriptor> services) { }

		protected internal override Func<IServiceProvider> ApplicationServiceFactory { get; set; }

		public Application()
		{
			ApplicationServiceFactory = UseBuiltInProviderHonouringServiceConfigurations;
		}

		private IServiceProvider UseBuiltInProviderHonouringServiceConfigurations()
		{
			Services = new ServiceCollection();
			PreServiceConfiguration(Services);
			try
			{
				return Services.BuildServiceProvider();
			}
			finally
			{
				PostServiceConfiguration(Services.ToList().AsReadOnly());
			}
		}
	}
}
