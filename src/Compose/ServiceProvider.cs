using Microsoft.Extensions.DependencyInjection;
using System;

namespace Compose
{
    public class ServiceProvider
    {
	    internal Func<IServiceProvider> ApplicationServiceFactory 
			= () => new ServiceCollection().BuildServiceProvider();

		private IServiceProvider _applicationServices;
		public IServiceProvider ApplicationServices
		{
			get { return _applicationServices ?? (_applicationServices = ApplicationServiceFactory()); }
			internal set { _applicationServices = value; }
		}

		internal IServiceCollection Services { get; set; }
	}
}
