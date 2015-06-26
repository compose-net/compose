using Microsoft.Framework.DependencyInjection;
using System;
using Xunit;

namespace Compose.Tests
{
    public class ScopeTests
    {
		public class TypeBased
		{
			[Fact]
			public void WhenResolvingTransientThenProviderReturnsMultipleInstances()
			{
				var app = new Fake.Executable();
				app.UseServices(services => services.AddTransient<IDependency, Dependency>());
				Assert.True(app.CanResolveMultipleInstances<IDependency>());
			}

			[Fact]
			public void WhenResolvingTransientTransitionalThenProviderReturnsMultipleInstances()
			{

				var app = new Fake.Executable();
				app.UseServices(services => services.AddTransient<IDependency, Dependency>().AsTransitional());
				Assert.True(app.CanResolveMultipleInstances<IDependency>());
			}

			[Fact]
			public void WhenResolvingSingletonThenProviderReturnsSingleInstance()
			{
				var app = new Fake.Executable();
				app.UseServices(services => services.AddSingleton<IDependency, Dependency>());
				Assert.False(app.CanResolveMultipleInstances<IDependency>());
			}

			[Fact]
			public void WhenResolvingSingletonTransitionalThenProviderReturnsSingleInstance()
			{
				var app = new Fake.Executable();
				app.UseServices(services => services.AddSingleton<IDependency, Dependency>().AsTransitional());
				Assert.False(app.CanResolveMultipleInstances<IDependency>());
			}
		}

		public class InstanceBased
		{
			[Fact]
			public void WhenResolvingInstanceThenProviderReturnsSingleInstance()
			{
				var app = new Fake.Executable();
				var instance = new Dependency();
				app.UseServices(services => services.AddInstance<IDependency>(instance));
				Assert.False(app.CanResolveMultipleInstances<IDependency>());
			}

			[Fact]
			public void WhenResolvingTransitionalInstanceThenProviderReturnsSingleInstance()
			{
				var app = new Fake.Executable();
				var instance = new Dependency();
				app.UseServices(services => services.AddInstance<IDependency>(instance).AsTransitional());
				Assert.False(app.CanResolveMultipleInstances<IDependency>());
			}
		}

		public class FactoryBased
		{
			[Fact]
			public void WhenTransientFactoryReturnsInstancesThenProviderReturnsInstances()
			{
				var app = new Fake.Executable();
				Func<IServiceProvider, IDependency> factory = provider => new Dependency();
				app.UseServices(services => services.AddTransient(factory));
				Assert.True(app.CanResolveMultipleInstances<IDependency>());
			}

			[Fact]
			public void WhenTransientTransitionalFactoryReturnsInstancesThenProviderReturnsInstances()
			{
				var app = new Fake.Executable();
				Func<IServiceProvider, IDependency> factory = provider => new Dependency();
				app.UseServices(services => services.AddTransient(factory).AsTransitional());
				Assert.True(app.CanResolveMultipleInstances<IDependency>());
			}

			[Fact]
			public void WhenSingletonFactoryReturnsInstancesThenProviderReturnsSingleInstance()
			{
				var app = new Fake.Executable();
				Func<IServiceProvider, IDependency> factory = provider => new Dependency();
				app.UseServices(services => services.AddSingleton(factory));
				Assert.False(app.CanResolveMultipleInstances<IDependency>());
			}

			[Fact]
			public void WhenSingletonTransitionalFactoryReturnsInstancesThenProviderReturnsSingleInstance()
			{
				var app = new Fake.Executable();
				Func<IServiceProvider, IDependency> factory = provider => new Dependency();
				app.UseServices(services => services.AddSingleton(factory).AsTransitional());
				Assert.False(app.CanResolveMultipleInstances<IDependency>());
			}
		}

		public interface IDependency { }

		private class Dependency : IDependency { }
    }
}
