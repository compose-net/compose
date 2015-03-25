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
				var service = new Dependency();
				services.AddInstance<IService1>(service);
				services.AddInstance<IService2>(service);
			});
			Action act = app.Execute;
			act.ShouldNotThrow<Exception>();
		}

		[Fact]
		public void CanTreatInstancesAsSingletons()
		{
			var app = new Fake.Application();
			var instance = new Dependency();
			app.UseServices(services =>
			{
				services.AddInstance<IService1>(instance);
			});
			app.OnExecute<Parent>(parent =>
			{
				parent.Service.Should().Be(instance);
				app.HostingServices.GetService<IService1>()
					.Should().Be(instance);
			});
			app.Execute();
		}

		private interface IService1 { }
		private interface IService2 { }
		private class Dependency : IService1, IService2 { }
		private class Parent
		{
			public IService1 Service { get; private set; }
			public Parent(IService1 service) { Service = service; }
		}
	}
}
