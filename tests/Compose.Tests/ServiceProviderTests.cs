using FluentAssertions;
using Microsoft.Framework.DependencyInjection;
using System;
using Xunit;

namespace Compose.Tests
{
	public class ServiceProviderTests
	{
		[Fact]
		public void CanHandleImplementationInstances()
		{
			var app = new Fake.Application();
			app.UseServices(services => services.AddInstance(typeof(string), "foo"));
			Action act = app.Execute;
			act.ShouldNotThrow<Exception>();
		}

		[Fact]
		public void CanHandleSameInstanceImplementingMultipleServices()
		{
			var app = new Fake.Application();
			app.UseServices(services =>
			{
				var service = new Service();
				services.AddInstance<IService1>(service);
				services.AddInstance<IService2>(service);
			});
			Action act = app.Execute;
			act.ShouldNotThrow<Exception>();
		}

		private interface IService1 { }
		private interface IService2 { }
		private class Service : IService1, IService2 { }
	}
}
