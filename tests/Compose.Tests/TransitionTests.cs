using FluentAssertions;
using Microsoft.Framework.DependencyInjection;
using System;
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
				dependency.Id.Should().Be(Type.Dependency);
			});
			app.Execute();
		}

		[Fact]
		public void CanTransitionService()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransitional<IDependency, Dependency>(); });
			app.OnExecute<IDependency>(dependency =>
			{
				dependency.Id.Should().Be(Type.Dependency);
				app.Transition<IDependency, OtherDependency>();
				dependency.Id.Should().Be(Type.OtherDependency);
			});
			app.Execute();
		}

		[Fact]
		public void CanTransitionSpecifcallyBoundService()
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
			app.Execute();
		}

		[Fact]
		public void CanTransitionAllBoundServices()
		{
			var app = new Fake.Application();
			app.UseServices(services =>
			{
				services.AddTransient<IDependency, Dependency>()
					.AsTransitional();
			});
			app.OnExecute<IDependency>(dependency =>
			{
				dependency.Id.Should().Be(Type.Dependency);
				app.Transition<IDependency, OtherDependency>();
				dependency.Id.Should().Be(Type.OtherDependency);
			});
			app.Execute();
		}

		[Fact]
		public void CanTransitionBackToOriginalService()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransitional<IDependency, Dependency>(); });
			app.OnExecute<IDependency>(dependency =>
			{
				app.Transition<IDependency, OtherDependency>();
				app.Transition<IDependency, Dependency>();
				dependency.Id.Should().Be(Type.Dependency);
			});
			app.Execute();
		}

		[Fact]
		public void CanPassThroughGenericArgumentsForGenericProxies()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient(typeof(IGenericDependency<>), typeof(GenericDependency<>)); });
			app.OnExecute<IGenericDependency<byte[]>>(dependency =>
			{
				dependency.Id.Should().Be(Type.GenericDependency);
			});
			app.Execute();
		}

		[Fact]
		public void CanTransitionUnresolvedService()
		{
			var app = new Fake.Application();
			app.UseServices(services => services.AddTransient<IDependency, Dependency>().AsTransitional());
			app.OnExecute(() =>
			{
				Action act = () => app.Transition<IDependency, OtherDependency>();
				act.ShouldNotThrow<Exception>();
			});
			app.Execute();
		}

		[Fact]
		public void CannotTransitionServicesAddedAfterMarker()
		{
			var app = new Fake.Application();
			app.UseServices(services =>
			{
				services
					.AsTransitional()
					.AddTransient<IDependency, Dependency>();
			});
			app.OnExecute(() =>
			{
				app.Transition<IDependency, OtherDependency>();
			});
			Action act = app.Execute;
			act.ShouldThrow<InvalidOperationException>();
		}

		[Fact]
		public void CanTransitionServicesAddedBeforeLastMarker()
		{
			var app = new Fake.Application();
			app.UseServices(services =>
			{
				services
					.AsTransitional()
					.AddTransient<IDependency, Dependency>()
					.AsTransitional();
			});
			app.OnExecute(() =>
			{
				app.Transition<IDependency, OtherDependency>()
				.Should().BeTrue();
			});
		}

		public enum Type { Dependency, OtherDependency, GenericDependency }

		public interface IDependency { Type Id { get; } }

		private class Dependency : IDependency { public Type Id { get; private set; } = Type.Dependency; }

		private class OtherDependency : IDependency { public Type Id { get; private set; } = Type.OtherDependency; }

		public interface IGenericDependency<T> { Type Id { get; } }

		private class GenericDependency<T> : IGenericDependency<T> { public Type Id { get; private set; } = Type.GenericDependency; }
	}
}
