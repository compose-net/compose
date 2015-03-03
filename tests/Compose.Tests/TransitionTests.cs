using FluentAssertions;
using Microsoft.Framework.DependencyInjection;
using Xunit;

namespace Compose.Tests
{
	public class TransitionTests
	{
		[Fact]
		public void CanResolveServiceAddedAsTransitional()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransitional<IDependency, Dependency>(); });
			app.OnExecute<IDependency>(dependency =>
			{
				dependency.Should().NotBeNull();
				dependency.Should().BeOfType<Dependency>();
			});
			app.Execute();
		}

		[Fact]
		public void CanResolveDirectServiceAddedAsTransitional()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransitional<IDependency, DefaultDependency>(); });
			app.OnExecute<IDependency>(dependency =>
			{
				dependency.Should().NotBeNull();
				dependency.Should().BeOfType<DefaultDependency>();
			});
			app.Execute();
		}

		[Fact]
		public void CanResolveFactoryForServiceAddedAsTransitional()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransitional<IDependency, Dependency>(); });
			app.OnExecute<IFactoryTransition<IDependency>>(factory =>
			{
				factory.Should().NotBeNull();
				factory.GetService().Should().NotBeNull();
				factory.GetService().Should().BeOfType<Dependency>();
			});
			app.Execute();
		}

		[Fact]
		public void CanTransitionUndirectService()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransitional<IDependency, Dependency>(); });
			app.OnExecute<IDependency>(dependency =>
			{
				app.Transition<IDependency, OtherDependency>();
				dependency.Should().BeOfType<Dependency>();
			});
			app.Execute();
		}

		[Fact]
		public void CanTransitionDirectService()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransitional<IDependency, DefaultDependency>(); });
			app.OnExecute<IDependency>(dependency =>
			{
				dependency.Should().BeOfType<DefaultDependency>();
				dependency.Id.Should().Be(Type.Dependency);
				app.Transition<IDependency, OtherDependency>();
				dependency.Id.Should().Be(Type.OtherDependency);
			});
			app.Execute();
		}

		[Fact]
		public void CanTransitionFactoryService()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransitional<IDependency, Dependency>(); });
			app.OnExecute<IFactoryTransition<IDependency>>(factory =>
			{
				factory.GetService().Id.Should().Be(Type.Dependency);
				app.Transition<IDependency, OtherDependency>();
				factory.GetService().Id.Should().Be(Type.OtherDependency);
			});
			app.Execute();
		}

		[Fact]
		public void CanTransitionFactoryWithoutTransitioningProducts()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransitional<IDependency, Dependency>(); });
			app.OnExecute<IFactoryTransition<IDependency>>(factory =>
			{
				var before = factory.GetService();
				app.Transition<IDependency, OtherDependency>();
				before.Id.Should().Be(Type.Dependency);
			});
			app.Execute();
		}

		[Fact]
		public void CanTransitionExternallyBoundDirectService()
		{
			var app = new Fake.Application();
			app.UseServices(services =>
			{
				services.AddTransient<IDependency, Dependency>()
					.WithTransitional<IDependency>();
			});

			app.OnExecute<IDependency>(dependency =>
			{
				dependency.Id.Should().Be(Type.Dependency);
				app.Transition<IDependency, OtherDependency>();
				dependency.Id.Should().Be(Type.OtherDependency);
			});
		}

		private enum Type { Dependency, OtherDependency }

		private interface IDependency { Type Id { get; } }

		private class Dependency : IDependency { public Type Id { get; private set; } = Type.Dependency; }

		private class OtherDependency : IDependency { public Type Id { get; private set; } = Type.OtherDependency; }

		private class DefaultDependency : DirectDefaultTransition<IDependency, Dependency>, IDependency
		{
			public Type Id { get { return Service.Id; } }
			public DefaultDependency(Dependency dependency) : base(dependency) { }
		}

		private class DirectDependency : DirectTransition<IDependency>, IDependency
		{
			public Type Id { get { return Service.Id; } }
			public DirectDependency(Dependency dependency) : base(dependency) { }
		}
	}
}
