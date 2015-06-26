using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Compose
{
    public class Application
    {
		public string Name { get; set; }

		public IServiceProvider ApplicationServices { get; set; }

		protected internal virtual void PreServiceConfiguration(IServiceCollection services) { }

        internal IServiceCollection Services { get; set; }

		protected internal virtual void PostServiceConfiguration(IReadOnlyCollection<ServiceDescriptor> services) { }
    }
}