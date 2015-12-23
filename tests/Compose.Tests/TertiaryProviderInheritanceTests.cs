using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestAttributes;

namespace Compose.Tests
{
	public class TertiaryProviderInheritanceTests
	{
		[Unit]
		public static void WhenAddingProviderForUnRegisteredServiceThenThrowsException()
		{
			Action act = () => new Application().UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.Implementation>());
			act.ShouldThrow<InvalidOperationException>();
		}

		[Unit]
		public static void WhenAddingProviderForClassThenThrowsException()
		{
			var application = new Application();
			application.UseServices(services => services.AddTransient<Fake.Implementation>());
			Action act = () => application.UseProvider<Fake.Implementation>(services => services.AddTransient<Fake.Implementation>());
			act.ShouldThrow<InvalidOperationException>();
		}

		private static readonly Action<IServiceCollection> AddFakeTransientService
			= services => services.AddTransient<Fake.Service, Fake.Implementation>();

		[Unit]
		public static void WhenAddingTertiaryProviderThenProviderContainingTertiaryServicesIsReturned()
		{
			var application = new Application();
			application.UseServices(AddFakeTransientService);
			application.UseProvider<Fake.Service>(AddFakeTransientService)
				.Should().NotBeNull();
		}

		[Unit]
		public static void WhenTertiaryProviderConfiguredToUseInternalProviderThenReturnsProviderContainingService()
		{
			var application = new Application();
			application.UseServices(AddFakeTransientService);
			application
				.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.Implementation>())
				.ApplicationServices.GetService<Fake.Service>()
				.Should().NotBeNull();
		}

		[Unit]
		public static void WhenTertiaryProviderConfiguredToUseCustomProviderThenApplicationServicesMatchesCustomProvider()
		{
			var provider = new Mock<IServiceProvider>();
			var emitter = new Mock<DynamicEmitter>();
			emitter.Setup(m => m.GetManagedDynamicProxy(It.IsAny<TypeInfo>())).Returns(new Mock<Fake.Service>().Object.GetType().GetTypeInfo());
			provider.Setup(m => m.GetService(typeof (DynamicEmitter))).Returns(emitter.Object);
			var application = new Application();
			application.UseServices(AddFakeTransientService);
			application.UseProvider<Fake.Service>(_ => provider.Object).ApplicationServices
				.Should().Be(provider.Object);
		}

		[Unit]
		public static void WhenTertiaryProviderAddedThenDoesNotConflictWithPrimaryServices()
		{
			var application = new Application();
			application.UseServices(services => services.AddTransient<Fake.Service, Fake.Implementation>());
			application.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.AlternativeImplementation>());

			application.ApplicationServices.GetRequiredService<Fake.Service>().ServiceType
				.Should().Be(typeof(Fake.Implementation));

			//https://twitter.com/Smudge202/status/674341628807651328
			//.Should().BeOfType<Fake.Implementation>();
		}

		[Unit]
		public static void WhenTertiaryProviderServiceDependsOnPrimaryServiceThenTertiaryServiceCanBeResolved()
		{
			var application = new Application();
			application.UseServices(services =>
			{
				services.AddTransient<Fake.Consumer, Fake.ConsumerImplementation>();
				services.AddTransient<Fake.Service, Fake.Implementation>();
			});
			application
				.UseProvider<Fake.Consumer>(services => services.AddTransient<Fake.Consumer, Fake.ConsumerImplementation>())
				.ApplicationServices.GetService<Fake.Consumer>()
				.Should().NotBeNull();
		}

		[Unit]
		public static void WhenTertiaryProviderInheritsTransientFactoryThenConsumerCanBeResolved()
		{
			var application = new Application();
			application.UseServices(services =>
			{
				services.AddTransient<Fake.Consumer, Fake.ConsumerImplementation>();
				services.AddTransient<Fake.Implementation>();
				services.AddTransient<Fake.Service>(provider => provider.GetRequiredService<Fake.Implementation>());
			});
			application
				.UseProvider<Fake.Consumer>(services => services.AddTransient<Fake.Consumer, Fake.ConsumerImplementation>())
				.ApplicationServices.GetService<Fake.Consumer>().Service
				.Should().NotBeNull();
		}

		[Unit]
		public static void WhenTertiaryProviderInheritsScopedFactoryThenConsumerCanBeResolved()
		{
			var application = new Application();
			application.UseServices(services =>
			{
				services.AddTransient<Fake.Consumer, Fake.ConsumerImplementation>();
				services.AddTransient<Fake.Implementation>();
				services.AddScoped<Fake.Service>(provider => provider.GetRequiredService<Fake.Implementation>());
			});
			application
				.UseProvider<Fake.Consumer>(services => services.AddTransient<Fake.Consumer, Fake.ConsumerImplementation>())
				.ApplicationServices.GetService<Fake.Consumer>().Service
				.Should().NotBeNull();
		}

		[Unit]
		public static void WhenTertiaryProviderInheritsSingletonFactoryThenSingletonIsHonouredAcrossProviders()
		{
			var application = new Application();
			application.UseServices(services =>
			{
				services.AddTransient<Fake.Consumer, Fake.ConsumerImplementation>();
				services.AddTransient<Fake.Implementation>();
				services.AddSingleton<Fake.Service>(provider => provider.GetRequiredService<Fake.Implementation>());
			});
			application
				.UseProvider<Fake.Consumer>(services => services.AddTransient<Fake.Consumer, Fake.ConsumerImplementation>())
				.ApplicationServices.GetRequiredService<Fake.Consumer>().Service
				.Should().Be(application.ApplicationServices.GetRequiredService<Fake.Service>());
		}

		[Unit]
		public static void WhenTertiaryProviderInheritsSingletonTypedThenSingletonIsHonouredAcrossProviders()
		{
			var application = new Application();
			application.UseServices(services =>
			{
				services.AddTransient<Fake.Consumer, Fake.ConsumerImplementation>();
				services.AddSingleton<Fake.Service, Fake.Implementation>();
			});
			application
				.UseProvider<Fake.Consumer>(services => services.AddTransient<Fake.Consumer, Fake.ConsumerImplementation>())
				.ApplicationServices.GetRequiredService<Fake.Consumer>().Service
				.Should().Be(application.ApplicationServices.GetRequiredService<Fake.Service>());
		}

		public class GivenTertiaryProviderAdded
		{

			[Unit]
			public static void WhenRequestingIEnumerableOfServiceOnPrimaryProviderThenReturnsPrimaryServiceOnly()
			{
				var application = new Application();
				application.UseServices(services => services.AddTransient<Fake.Service, Fake.Implementation>());
				application.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.AlternativeImplementation>());

				var results = application.ApplicationServices.GetRequiredService<IEnumerable<Fake.Service>>();

				results.Should().HaveCount(1);
				results.Single().ServiceType.Should().Be(typeof(Fake.Implementation));
			}

			[Unit]
			public static void WhenRequestingIEnumerableOfServiceOnTertiaryProviderThenReturnsTertiaryServiceOnly()
			{
				var application = new Application();
				application.UseServices(services => services.AddTransient<Fake.Service, Fake.Implementation>());
				var tertiaryProvider = application.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.AlternativeImplementation>());

				var results = tertiaryProvider.ApplicationServices.GetRequiredService<IEnumerable<Fake.Service>>();

				results.Should().HaveCount(2);
				results.Should().ContainSingle(x => x.ServiceType == typeof(Fake.Implementation));
				results.Should().ContainSingle(x => x.ServiceType == typeof(Fake.AlternativeImplementation));
			}
		}
	}
}
