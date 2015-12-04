using System;
using Microsoft.Extensions.DependencyInjection;

namespace Compose
{
    public class Application
    {
		public IServiceProvider ApplicationServices { get; internal set; }
			= new ServiceCollection().BuildServiceProvider();

		internal ServiceCollection Services { get; set; }
	}
}
