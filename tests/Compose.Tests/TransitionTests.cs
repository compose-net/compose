using Microsoft.Extensions.DependencyInjection;
using System;
using TestAttributes;
using Xunit;

namespace Compose.Tests
{
	public class TransitionTests
	{
		[Unit]
		public void CanResolveServicesWhenAddedAsTransitional()
		{
			var app = new Fake.Executable();
			app.UseServices(services => services.AddTransitional<IDependency, Dependency>());
			app.OnExecute<IDependency>(dependency =>
			{
				Assert.NotNull(dependency);
				Assert.Equal(Type.Dependency, dependency.Id);
			});
			app.Execute();
		}

		[Unit]
		public void CanResolveServicesWhenWithTransitional()
		{
			var app = new Fake.Executable();
			app.UseServices(services =>
			{
				services
					.AddTransient<IDependency, Dependency>()
					.WithTransitional<IDependency>();
			});
			app.OnExecute<IDependency>(dependency =>
			{
				Assert.NotNull(dependency);
				Assert.Equal(Type.Dependency, dependency.Id);
			});
			app.Execute();
		}

		[Unit]
		public void CanResolveServicesWhenAsTransitional()
		{
			var app = new Fake.Executable();
			app.UseServices(services =>
			{
				services
					.AddTransient<IDependency, Dependency>()
					.AsTransitional();
			});
			app.OnExecute<IDependency>(dependency =>
			{
				Assert.NotNull(dependency);
				Assert.Equal(Type.Dependency, dependency.Id);
			});
			app.Execute();
		}

		[Unit]
		public void CanResolveServicesIndirectlyWhenAddedAsTransitional()
		{
			var app = new Fake.Executable();
			app.UseServices(services =>
			{
				services.AddTransient<IConsumer, Consumer>();
				services.AddTransitional<IDependency, Dependency>();
			});
			app.OnExecute<IConsumer>(consumer =>
			{
				Assert.NotNull(consumer);
				Assert.Equal(Type.Dependency, consumer.DependencyId);
			});
			app.Execute();
		}

		[Unit]
		public void CanResolveServicesIndirectlyWhenWithTransitional()
		{
			var app = new Fake.Executable();
			app.UseServices(services =>
			{
				services
					.AddTransient<IConsumer, Consumer>()
					.AddTransient<IDependency, Dependency>()
					.WithTransitional<IDependency>();
			});
			app.OnExecute<IConsumer>(consumer =>
			{
				Assert.NotNull(consumer);
				Assert.Equal(Type.Dependency, consumer.DependencyId);
			});
			app.Execute();
		}

		[Unit]
		public void CanResolveServicesIndirectlyWhenAsTransitional()
		{
			var app = new Fake.Executable();
			app.UseServices(services =>
			{
				services
					.AddTransient<IDependency, Dependency>()
					.AsTransitional()
					.AddTransient<IConsumer, Consumer>();
			});
			app.OnExecute<IConsumer>(consumer =>
			{
				Assert.NotNull(consumer);
				Assert.Equal(Type.Dependency, consumer.DependencyId);
			});
			app.Execute();
		}

		[Unit]
		public void CanTransitionService()
		{
			var app = new Fake.Executable();
			app.UseServices(services => 
			{
				services
					.AddTransitional<IDependency, Dependency>()
					.AddTransient<OtherDependency>();
			});
			app.OnExecute<IDependency>(dependency =>
			{
				Assert.Equal(Type.Dependency, dependency.Id);
				app.Transition<IDependency, OtherDependency>();
				Assert.Equal(Type.OtherDependency, dependency.Id);
			});
			app.Execute();
		}

		[Unit]
		public void CanTransitionSpecificallyBoundService()
		{
			var app = new Fake.Executable();
			app.UseServices(services =>
			{
				services
					.AddTransient<IDependency, Dependency>()
					.AddTransient<OtherDependency>()
					.WithTransitional<IDependency>();
			});
			app.OnExecute<IDependency>(dependency =>
			{
				Assert.Equal(Type.Dependency, dependency.Id);
				app.Transition<IDependency, OtherDependency>();
				Assert.Equal(Type.OtherDependency, dependency.Id);
			});
			app.Execute();
		}

		[Unit]
		public void CanTransitionAllBoundServices()
		{
			var app = new Fake.Executable();
			app.UseServices(services =>
			{
				services
					.AddTransient<IDependency, Dependency>()
					.AddTransient<OtherDependency>()
					.AsTransitional();
			});
			app.OnExecute<IDependency>(dependency =>
			{
				Assert.Equal(Type.Dependency, dependency.Id);
				app.Transition<IDependency, OtherDependency>();
				Assert.Equal(Type.OtherDependency, dependency.Id);
			});
			app.Execute();
		}

		[Unit]
		public void CanTransitionBackToOriginalService()
		{
			var app = new Fake.Executable();
			app.UseServices(services =>
			{
				services
					.AddTransitional<IDependency, Dependency>()
					.AddTransient<OtherDependency>();
			});
			app.OnExecute<IDependency>(dependency =>
			{
				app.Transition<IDependency, OtherDependency>();
				app.Transition<IDependency, Dependency>();
				Assert.Equal(Type.Dependency, dependency.Id);
			});
			app.Execute();
		}

		[Unit]
		public void CanPassThroughGenericArgumentsForGenericProxies()
		{
			var app = new Fake.Executable();
			app.UseServices(services => { services.AddTransient(typeof(IGenericDependency<>), typeof(GenericDependency<>)); });
			app.OnExecute<IGenericDependency<byte[]>>(dependency =>
			{
				Assert.Equal(Type.GenericDependency, dependency.Id);
			});
			app.Execute();
		}

		[Unit]
		public void CanNotTransitionToUnresolvableService()
		{
			var app = new Fake.Executable();
			app.UseServices(services => services.AddTransient<IDependency, Dependency>().AsTransitional());
			app.OnExecute<IDependency>(dependency =>
			{
				Action act = () => app.Transition<IDependency, UnresolvableDependency>();
				Assert.IsType<InvalidOperationException>(Record.Exception(act));
			});
			app.Execute();
		}

		[Unit]
		public void CannotTransitionServicesAddedAfterMarker()
		{
			var app = new Fake.Executable();
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
			Assert.IsType(typeof(InvalidOperationException), Record.Exception(act));
		}

		[Unit]
		public void CanTransitionServicesAddedBeforeLastMarker()
		{
			var app = new Fake.Executable();
			app.UseServices(services =>
			{
				services
					.AsTransitional()
					.AddTransient<IDependency, Dependency>()
					.AsTransitional();
			});
			app.OnExecute(() =>
			{
				Assert.Null(Record.Exception(() => app.Transition<IDependency, OtherDependency>()));
			});
		}

		[Unit]
		public void CanTransitionIndirectService()
		{
			var app = new Fake.Executable();
			app.UseServices(services => services
				.AddTransitional<IDependency, Dependency>()
				.AddTransient<IConsumer, Consumer>()
				.AddTransient<OtherDependency, OtherDependency>()
			);
			app.OnExecute<IConsumer>(consumer =>
			{
				Assert.Equal(Type.Dependency, consumer.DependencyId);
				app.Transition<IDependency, OtherDependency>();
				Assert.Equal(Type.OtherDependency, consumer.DependencyId);
			});
			app.Execute();
		}

		public enum Type { Dependency, OtherDependency, GenericDependency }

		public interface IDependency { Type Id { get; } }

		internal class Dependency : IDependency { public Type Id { get; private set; } = Type.Dependency; }

		public interface IOtherDependency : IDependency { }

		internal class OtherDependency : IOtherDependency { public Type Id { get; private set; } = Type.OtherDependency; }

		public interface IGenericDependency<T> { Type Id { get; } }

		internal class GenericDependency<T> : IGenericDependency<T> { public Type Id { get; private set; } = Type.GenericDependency; }

		public interface IConsumer { Type DependencyId { get; } }

		internal class Consumer : IConsumer
		{
			private readonly IDependency _dependency;

			public Consumer(IDependency dependency)
			{
				_dependency = dependency;
			}

			public Type DependencyId { get { return _dependency.Id; } }
		}

		internal class UnresolvableDependency : IDependency
		{
			public UnresolvableDependency(IConsumer consumer) { /* don't bind IConsumer */ }

			public Type Id { get; } = Type.OtherDependency;
		}
	}
}
