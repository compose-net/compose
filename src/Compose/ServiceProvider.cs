using Microsoft.Extensions.DependencyInjection;
using System;

namespace Compose
{
    public class ServiceProvider
	{
		public IServiceProvider ApplicationServices { get; internal set; }
		= new ServiceCollection().BuildServiceProvider();

		internal IServiceCollection Services { get; set; }
	}
}
