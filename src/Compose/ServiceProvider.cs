using Microsoft.Extensions.DependencyInjection;
using System;

namespace Compose
{
	public abstract class ServiceProvider
	{
		protected internal abstract Func<IServiceProvider> ApplicationServiceFactory { get; set; }

		private IServiceProvider _applicationServices;
		public IServiceProvider ApplicationServices
		{
			get { return _applicationServices ?? (_applicationServices = ApplicationServiceFactory()); }
			internal set { _applicationServices = value; }
		}

		internal IServiceCollection Services { get; set; }
	}
}
