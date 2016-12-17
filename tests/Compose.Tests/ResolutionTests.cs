using Microsoft.Extensions.DependencyInjection;
using System;
using TestAttributes;
using Xunit;

namespace Compose.Tests
{
	public class ResolutionTests
	{
		private interface IDependency { }

		private interface IDependency2 { }

		private class Dependency : IDependency { }

		[Unit]
		public void WhenResolvingUnboundServiceThenThrowsDescriptiveException()
		{
			var app = new Application();

			app.UseServices(services => services.Add(ServiceDescriptor.Transient<IDependency, Dependency>()));

			Action act = () => app.ApplicationServices.GetRequiredService<IDependency2>();
			Assert.IsType<InvalidOperationException>(Record.Exception(act));
		}

		private interface IService<T> { }

		private class Service<T> : IService<T> { }

		[Unit]
		public void WhenMultipleTransientImplementationsRegisteredForGenericSerivceThenCanResolve()
		{
			var app = new Application();

			app.UseServices(services =>
			{
				services.AddTransient<IService<string>, Service<string>>();
				services.AddTransient<IService<int>, Service<int>>();
			});

			Action act = () => app.ApplicationServices.GetRequiredService<IService<string>>();
			Assert.Null(Record.Exception(act));
			act = () => app.ApplicationServices.GetRequiredService<IService<int>>();
			Assert.Null(Record.Exception(act));
			act = () => app.ApplicationServices.GetRequiredService<IService<object>>();
			Assert.NotNull(Record.Exception(act));
		}

		[Unit]
		public void WhenMultipleSingletonImplementationsRegisteredForGenericSerivceThenCanResolve()
		{
			var app = new Application();

			app.UseServices(services =>
			{
				services.AddSingleton<IService<string>, Service<string>>();
				services.AddSingleton<IService<int>, Service<int>>();
			});

			Action act = () => app.ApplicationServices.GetRequiredService<IService<string>>();
			Assert.Null(Record.Exception(act));
			act = () => app.ApplicationServices.GetRequiredService<IService<int>>();
			Assert.Null(Record.Exception(act));
			act = () => app.ApplicationServices.GetRequiredService<IService<object>>();
			Assert.NotNull(Record.Exception(act));
		}

		[Unit]
		public void WhenAddingClosedAndOpenGenericTransientImplementationsForGenericServiceThenCanResolve()
		{
			var app = new Application();

			app.UseServices(services =>
			{
				services.AddTransient(typeof(IService<>), typeof(Service<>));
				services.AddTransient<IService<int>, Service<int>>();
			});

			Action act = () => app.ApplicationServices.GetRequiredService<IService<string>>();
			Assert.Null(Record.Exception(act));
			act = () => app.ApplicationServices.GetRequiredService<IService<int>>();
			Assert.Null(Record.Exception(act));
		}

		[Unit]
		public void WhenAddingClosedAndOpenGenericSingletonImplementationsForGenericServiceThenCanResolve()
		{
			var app = new Application();

			app.UseServices(services =>
			{
				services.AddSingleton(typeof(IService<>), typeof(Service<>));
				services.AddSingleton<IService<int>, Service<int>>();
			});

			Action act = () => app.ApplicationServices.GetRequiredService<IService<string>>();
			Assert.Null(Record.Exception(act));
			act = () => app.ApplicationServices.GetRequiredService<IService<int>>();
			Assert.Null(Record.Exception(act));
		}

		[Unit]
		public void WhenAddingSameServiceTypeMultipleTimesThenCanResolve()
		{
			var app = new Application();

			Action act = () => app.UseServices(services =>
			{
				services.Add(new ServiceDescriptor(typeof(IService<>), typeof(Service<>), ServiceLifetime.Transient));
				services.Add(new ServiceDescriptor(typeof(IService<>), typeof(Service<>), ServiceLifetime.Transient));
			});
			Assert.Null(Record.Exception(act));
		}
	}
}
