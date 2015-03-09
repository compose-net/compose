using FluentAssertions;
using System;
using Xunit;

namespace Compose.Tests
{
	public class SnapshotTests
	{
		[Fact]
		public void CanSnapshot()
		{
			var app = new Fake.Application();
			app.UseServices(services => services.AddTransitional<IDependency, Dependency1>());
			Action act = app.Snapshot;
			act.ShouldNotThrow<Exception>();
		}

		[Fact]
		public void CanSnapshotWithoutServices()
		{
			var app = new Fake.Application();
			Action act = app.Snapshot;
			act.ShouldNotThrow<Exception>();
		}

		[Fact]
		public void CanRestore()
		{
			var app = new Fake.Application();
			app.UseServices(services => services.AddTransitional<IDependency, Dependency1>());
			Action act = app.Restore;
			act.ShouldNotThrow<Exception>();
		}

		[Fact]
		public void CanRestoreWithoutServices()
		{
			var app = new Fake.Application();
			Action act = app.Restore;
			act.ShouldNotThrow<Exception>();
		}

		[Fact]
		public void CanRestoreExplicitlySnapshottedTransitionedServices()
		{
			var app = new Fake.Application();
			app.UseServices(services => services.AddTransitional<IDependency, Dependency1>());
			app.OnExecute<IDependency>(dependency =>
			{
				app.Snapshot();
				app.Transition<IDependency, Dependency2>();
				dependency.Id.Should().Be(Type.Two);
				app.Restore();
				dependency.Id.Should().Be(Type.One);
			});
			app.Execute();
		}

		[Fact]
		public void CanRestoreImplicitlySnapshottedTransitionedServices()
		{
			var app = new Fake.Application();
			app.UseServices(services => services.AddTransitional<IDependency, Dependency1>());
			app.OnExecute<IDependency>(dependency =>
			{
				app.Transition<IDependency, Dependency2>();
				dependency.Id.Should().Be(Type.Two);
				app.Restore();
				dependency.Id.Should().Be(Type.One);
			});
			app.Execute();
		}

		[Fact]
		public void CanRestoreLatestSnapshot()
		{
			var app = new Fake.Application();
			app.UseServices(services => services.AddTransitional<IDependency, Dependency1>());
			app.OnExecute<IDependency>(dependency =>
			{
				app.Snapshot();
				app.Transition<IDependency, Dependency2>();
				app.Snapshot();
				app.Transition<IDependency, Dependency3>();
				app.Restore();
				dependency.Id.Should().Be(Type.Two);
			});
			app.Execute();
		}

		public enum Type { One, Two, Three }

		public interface IDependency { Type Id { get; } }

		private sealed class Dependency1 : IDependency
		{
			public Type Id { get { return Type.One; } }
		}

		private sealed class Dependency2 : IDependency
		{
			public Type Id { get { return Type.Two; } }
		}

		private sealed class Dependency3 : IDependency
		{
			public Type Id { get { return Type.Three; } }
		}
	}
}
