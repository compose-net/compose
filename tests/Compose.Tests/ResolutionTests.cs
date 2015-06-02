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

			Action act = () => app.ApplicationServices.GetRequiredService<IDependency2>();
			Assert.IsType<InvalidOperationException>(Record.Exception(act));
		}

		private interface IService<T> { }

		private class Service<T> : IService<T> { }

		[Fact]
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

		[Fact]
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

		[Fact]
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

		[Fact]
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

		[Fact]
		public void WhenAddingSameServiceTypeMultipleTimesThenCanResolve()
		{
			var app = new Application();

			Action act = () => app.UseServices(services =>
			{
				services.Add(new ServiceDescriptor(typeof(IService<>), null));
				services.Add(new ServiceDescriptor(typeof(IService<>), null));
			});
			Assert.Null(Record.Exception(act));
		}
	}
}
