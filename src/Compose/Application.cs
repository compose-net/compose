using Microsoft.Framework.DependencyInjection;
using System;

namespace Compose
{
    public class Application
    {
		public string Name { get; set; }

		public IServiceProvider ApplicationServices { get; set; }

        internal ServiceCollection Services { get; set; }
    }
}