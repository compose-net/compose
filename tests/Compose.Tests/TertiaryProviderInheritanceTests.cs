using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Reflection;
using Xunit;

namespace Compose.Tests
{
	public class TertiaryProviderInheritanceTests
	{
		[Fact]
		public static void WhenAddingProviderForUnRegisteredServiceThenThrowsException()
		{
			Action act = () => new Application().UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.Implementation>());
			act.ShouldThrow<InvalidOperationException>();
		}

		[Fact]
		public static void WhenAddingProviderForClassThenThrowsException()
		{
			var application = new Application();
			application.UseServices(services => services.AddTransient<Fake.Implementation>());
			Action act = () => application.UseProvider<Fake.Implementation>(services => services.AddTransient<Fake.Implementation>());
			act.ShouldThrow<InvalidOperationException>();
		}

		private static readonly Action<IServiceCollection> AddFakeTransientService
			= services => services.AddTransient<Fake.Service, Fake.Implementation>();

		[Fact]
		public static void WhenAddingTertiaryProviderThenProviderContainingTertiaryServicesIsReturned()
		{
			var application = new Application();
			application.UseServices(AddFakeTransientService);
			application.UseProvider<Fake.Service>(AddFakeTransientService)
				.Should().NotBeNull();
		}

		[Fact]
		public static void WhenTertiaryProviderConfiguredToUseInternalProviderThenReturnsProviderContainingService()
		{
			var application = new Application();
			application.UseServices(AddFakeTransientService);
			application
				.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.Implementation>())
				.ApplicationServices.GetService<Fake.Service>()
				.Should().NotBeNull();
		}

		[Fact]
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

		[Fact]
		public static void WhenTertiaryProviderAddedThenDoesNotConflictWithPrimaryServices()
		{
			var application = new Application();
			application.UseServices(services => services.AddTransient<Fake.Service, Fake.Implementation>());
			application.UseProvider<Fake.Service>(services => services.AddTransient<Fake.Service, Fake.AlternativeImplementation>());

			var x = application.ApplicationServices.GetRequiredService<Fake.Service>();

			application.ApplicationServices.GetRequiredService<Fake.Service>().ServiceType
				.Should().BeOfType<Fake.Implementation>();
		}

		[Fact]
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

		[Fact]
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

		[Fact]
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

		[Fact]
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

		[Fact]
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
	}
}
