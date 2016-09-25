using Microsoft.Extensions.DependencyInjection;
using System;
using TestAttributes;
using Xunit;

namespace Compose.Tests
{
	public class ServiceProviderTests
	{
		[Fact]
		public void CanHandleImplementationInstances()
		{
			var app = new Fake.Executable();
			app.UseServices(services => services.AddSingleton(typeof(string), "foo"));
			Action act = app.Execute;
			Assert.Null(Record.Exception(act));
		}

		[Fact]
		public void CanHandleSameInstanceImplementingMultipleServices()
		{
			var app = new Fake.Executable();
			app.UseServices(services =>
			{
				var service = new Service();
				services.AddSingleton<IService1>(service);
				services.AddSingleton<IService2>(service);
			});
			Action act = app.Execute;
			Assert.Null(Record.Exception(act));
		}

		[Fact]
		public void CanTreatInstancesAsSingletons()
		{
			var app = new Fake.Executable();
			var instance = new Service();
			app.UseServices(services =>
			{
				services.AddSingleton<IService1>(instance);
			});
			app.OnExecute<IService1>(parent =>
			{
				Assert.Equal(instance, parent);
				Assert.Equal(instance, app.ApplicationServices.GetService<IService1>());
			});
			app.Execute();
		}

		[Fact]
		public void CanUseCustomServiceProvider()
		{
			var app = new Fake.Executable();
			app.UseServices(services => new CustomServiceProvider());
			app.OnExecute<IService1>(service => { });
			Action act = app.Execute;
			Assert.IsType<NotImplementedException>(Record.Exception(act));
		}

		[Fact]
		public void CanResolveSingletonsIndirectly()
		{
			var app = new Fake.Executable();
			app.UseServices(services => services
				.AddTransient<IConsumer, Consumer>()
				.AddSingleton<IDependency, Dependency>()
			);
			app.OnExecute<IConsumer>(consumer =>
			{
				var newConsumer = app.ApplicationServices.GetRequiredService<IConsumer>();
				Assert.Equal(consumer.Id, newConsumer.Id);
				Assert.NotEqual(consumer, newConsumer);
			});
		}

		private interface IService1 { }
		private interface IService2 { }
		private class Service : IService1, IService2 { }

		private interface IConsumer { Guid Id { get; } }
		private class Consumer : IConsumer
		{
			private readonly IDependency _dependency;
			public Consumer(IDependency dependency)
			{
				_dependency = dependency;
			}

			public Guid Id { get { return _dependency.Id; } }
		}

		private interface IDependency { Guid Id { get; } }
		private class Dependency : IDependency
		{
			public Guid Id { get; } = Guid.NewGuid();
		}

		private class CustomServiceProvider : IServiceProvider
		{
			public object GetService(Type serviceType)
			{
				throw new NotImplementedException();
			}
		}
	}
}
