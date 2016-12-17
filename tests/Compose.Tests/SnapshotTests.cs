﻿using Microsoft.Extensions.DependencyInjection;
using System;
using TestAttributes;
using Xunit;

namespace Compose.Tests
{
	public class SnapshotTests
	{
		[Unit]
		public void CanSnapshot()
		{
			var app = new Fake.Executable();
			app.UseServices(services => services.AddTransitional<IDependency, Dependency1>());
			Action act = app.Snapshot;
			Assert.Null(Record.Exception(act));
		}

		[Unit]
		public void CanSnapshotWithoutServices()
		{
			var app = new Fake.Executable();
			Action act = app.Snapshot;
			Assert.Null(Record.Exception(act));
		}

		[Unit]
		public void CanRestore()
		{
			var app = new Fake.Executable();
			app.UseServices(services => services.AddTransitional<IDependency, Dependency1>());
			Action act = app.Restore;
			Assert.Null(Record.Exception(act));
		}

		[Unit]
		public void CanRestoreWithoutServices()
		{
			var app = new Fake.Executable();
			Action act = app.Restore;
			Assert.Null(Record.Exception(act));
		}

		[Unit]
		public void CanRestoreExplicitlySnapshottedTransitionedServices()
		{
			var app = new Fake.Executable();
			app.UseServices(services =>
			{
				services.AddTransitional<IDependency, Dependency1>();
				services.AddTransient<Dependency2>();
            });
			app.OnExecute<IDependency>(dependency =>
			{
				app.Snapshot();
				app.Transition<IDependency, Dependency2>();
				Assert.Equal(Type.Two, dependency.Id);
				app.Restore();
				Assert.Equal(Type.One, dependency.Id);
			});
			app.Execute();
		}

		[Unit]
		public void CanRestoreImplicitlySnapshottedTransitionedServices()
		{
			var app = new Fake.Executable();
			app.UseServices(services =>
			{
				services.AddTransitional<IDependency, Dependency1>();
				services.AddTransient<Dependency2>();
			});
			app.OnExecute<IDependency>(dependency =>
			{
				app.Transition<IDependency, Dependency2>();
				Assert.Equal(Type.Two, dependency.Id);
				app.Restore();
				Assert.Equal(Type.One, dependency.Id);
			});
			app.Execute();
		}

		[Unit]
		public void CanRestoreLatestSnapshot()
		{
			var app = new Fake.Executable();
			app.UseServices(services =>
			{
				services.AddTransitional<IDependency, Dependency1>();
				services.AddTransient<Dependency2>();
				services.AddTransient<Dependency3>();
			});
			app.OnExecute<IDependency>(dependency =>
			{
				app.Snapshot();
				app.Transition<IDependency, Dependency2>();
				app.Snapshot();
				app.Transition<IDependency, Dependency3>();
				app.Restore();
				Assert.Equal(Type.Two, dependency.Id);
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
