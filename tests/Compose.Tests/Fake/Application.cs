using Microsoft.Framework.DependencyInjection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Compose.Tests.Fake
{
    internal sealed class Application : Compose.Application
    {
		public ReadOnlyCollection<ServiceDescriptor> PreConfiguredServices { get; private set; }
		public IEnumerable<ServiceDescriptor> ServicesToAppendPreConfiguration;
		protected internal override void PreServiceConfiguration(IServiceCollection services)
		{
			PreConfiguredServices = new List<ServiceDescriptor>(services).AsReadOnly();
			if ((ServicesToAppendPreConfiguration?.Count() ?? 0) > 0)
				services.TryAdd(ServicesToAppendPreConfiguration);
			base.PreServiceConfiguration(services);
		}

		public ReadOnlyCollection<ServiceDescriptor> PostConfiguredServices { get; private set; }
		protected internal override void PostServiceConfiguration(IReadOnlyCollection<ServiceDescriptor> services)
		{
			PostConfiguredServices = new List<ServiceDescriptor>(services).AsReadOnly();
			base.PostServiceConfiguration(services);
		}
	}
}
