using Microsoft.Framework.DependencyInjection;
using System;
using Xunit;

namespace Compose.Tests
{
	public class ResolutionTests
	{
		private interface IDependency { }

		private interface IDependency2 { }

		private class Dependency : IDependency { }

		[Fact]
		public void WhenResolvingUnboundServiceThenThrowsDescriptiveException()
		{
			var app = new Application();

			app.UseServices(services => services.Add(ServiceDescriptor.Transient<IDependency, Dependency>()));

			Action act = () => app.GetRequiredService<IDependency2>();
			Assert.IsType<InvalidOperationException>(Record.Exception(act));
		}
	}
}
