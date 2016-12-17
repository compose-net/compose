using Microsoft.Extensions.DependencyInjection;
using System;
using TestAttributes;
using Xunit;

namespace Compose.Tests
{
    public class ScopeTests
    {
		public class TypeBased
		{
			[Unit]
			public void WhenResolvingTransientThenProviderReturnsMultipleInstances()
			{
				var app = new Fake.Executable();
				app.UseServices(services => services.AddTransient<IDependency, Dependency>());
				Assert.True(app.CanResolveMultipleInstances<IDependency>());
			}

			[Unit]
			public void WhenResolvingTransientTransitionalThenProviderReturnsMultipleInstances()
			{

				var app = new Fake.Executable();
				app.UseServices(services => services.AddTransient<IDependency, Dependency>().AsTransitional());
				Assert.True(app.CanResolveMultipleInstances<IDependency>());
			}

			[Unit]
			public void WhenResolvingSingletonThenProviderReturnsSingleInstance()
			{
				var app = new Fake.Executable();
				app.UseServices(services => services.AddSingleton<IDependency, Dependency>());
				Assert.False(app.CanResolveMultipleInstances<IDependency>());
			}

			[Unit]
			public void WhenResolvingSingletonTransitionalThenProviderReturnsSingleInstance()
			{
				var app = new Fake.Executable();
				app.UseServices(services => services.AddSingleton<IDependency, Dependency>().AsTransitional());
				Assert.False(app.CanResolveMultipleInstances<IDependency>());
			}
		}

		public class InstanceBased
		{
			[Unit]
			public void WhenResolvingInstanceThenProviderReturnsSingleInstance()
			{
				var app = new Fake.Executable();
				var instance = new Dependency();
				app.UseServices(services => services.AddSingleton<IDependency>(instance));
				Assert.False(app.CanResolveMultipleInstances<IDependency>());
			}

			[Unit]
			public void WhenResolvingTransitionalInstanceThenProviderReturnsSingleInstance()
			{
				var app = new Fake.Executable();
				var instance = new Dependency();
				app.UseServices(services => services.AddSingleton<IDependency>(instance).AsTransitional());
				Assert.False(app.CanResolveMultipleInstances<IDependency>());
			}
		}

		public class FactoryBased
		{
			[Unit]
			public void WhenTransientFactoryReturnsInstancesThenProviderReturnsInstances()
			{
				var app = new Fake.Executable();
				Func<IServiceProvider, IDependency> factory = provider => new Dependency();
				app.UseServices(services => services.AddTransient(factory));
				Assert.True(app.CanResolveMultipleInstances<IDependency>());
			}

			[Unit]
			public void WhenTransientTransitionalFactoryReturnsInstancesThenProviderReturnsInstances()
			{
				var app = new Fake.Executable();
				Func<IServiceProvider, IDependency> factory = provider => new Dependency();
				app.UseServices(services => services.AddTransient(factory).AsTransitional());
				Assert.True(app.CanResolveMultipleInstances<IDependency>());
			}

			[Unit]
			public void WhenSingletonFactoryReturnsInstancesThenProviderReturnsSingleInstance()
			{
				var app = new Fake.Executable();
				Func<IServiceProvider, IDependency> factory = provider => new Dependency();
				app.UseServices(services => services.AddSingleton(factory));
				Assert.False(app.CanResolveMultipleInstances<IDependency>());
			}

			[Unit]
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
