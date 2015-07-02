using Microsoft.Framework.DependencyInjection;
using System;
using TestAttributes;
using Xunit;

namespace Compose.Tests
{
    public class DisposableTests
	{
		[Unit]
		public void WhenTransitioningAwayFromDirectlyImplementedDisposableThenDisposesCurrentService()
		{
			var app = new Fake.Executable();
			app.UseServices(services => services
				.AddTransitional<IDependency, DirectlyDisposableDependency>()
				.AddTransient<Dependency, Dependency>()
			);
			app.OnExecute<IDependency>(dependency =>
			{
				Action act = app.Transition<IDependency, Dependency>;
				Assert.Throws<NotImplementedException>(act);
			});
		}

		[Unit]
		public void WhenTransitioningAwayFromIndirectlyImplementedDisposableThenDisposesCurrentService()
		{
			var app = new Fake.Executable();
			app.UseServices(services => services
				.AddTransitional<IDependency, IndirectlyDisposableDependency>()
				.AddTransient<Dependency, Dependency>()
			);
			app.OnExecute<IDependency>(dependency =>
			{
				Action act = app.Transition<IDependency, Dependency>;
				Assert.Throws<NotImplementedException>(act);
			});
		}

		[Unit]
		public void WhenSnapshottingAwayFromDirectlyImplementedDisposableThenDisposesCurrentService()
		{
			var app = new Fake.Executable();
			app.UseServices(services => services
				.AddTransitional<IDependency, DirectlyDisposableDependency>()
				.AddTransient<Dependency, Dependency>()
			);
			app.OnExecute<IDependency>(dependency =>
			{
				Action act = app.Snapshot;
				Assert.Throws<NotImplementedException>(act);
			});
		}

		[Unit]
		public void WhenSnapshottingAwayFromIndirectlyImplementedDisposableThenDisposesCurrentService()
		{
			var app = new Fake.Executable();
			app.UseServices(services => services
				.AddTransitional<IDependency, IndirectlyDisposableDependency>()
				.AddTransient<Dependency, Dependency>()
			);
			app.OnExecute<IDependency>(dependency =>
			{
				Action act = app.Snapshot;
				Assert.Throws<NotImplementedException>(act);
			});
		}

		[Unit]
		public void WhenRestoringAwayFromDirectlyImplementedDisposableThenDisposesCurrentService()
		{
			var app = new Fake.Executable();
			app.UseServices(services => services
				.AddTransitional<IDependency, DirectlyDisposableDependency>()
				.AddTransient<Dependency, Dependency>()
			);
			app.OnExecute<IDependency>(dependency =>
			{
				Action act = app.Restore;
				Assert.Throws<NotImplementedException>(act);
			});
		}

		[Unit]
		public void WhenRestoringAwayFromIndirectlyImplementedDisposableThenDisposesCurrentService()
		{
			var app = new Fake.Executable();
			app.UseServices(services => services
				.AddTransitional<IDependency, IndirectlyDisposableDependency>()
				.AddTransient<Dependency, Dependency>()
			);
			app.OnExecute<IDependency>(dependency =>
			{
				Action act = app.Restore;
				Assert.Throws<NotImplementedException>(act);
			});
		}

		public interface IDependency { }

		private class Dependency : IDependency { }

		private class DirectlyDisposableDependency : IDependency, IDisposable
		{
			public void Dispose() { throw new NotImplementedException(); }
		}

		private interface IDisposableDependency : IDependency, IDisposable { }

		private class IndirectlyDisposableDependency : IDisposableDependency
		{
			public void Dispose() { throw new NotImplementedException(); }
		}
	}
}
