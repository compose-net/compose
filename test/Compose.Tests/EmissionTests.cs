using FluentAssertions;
using Microsoft.Framework.DependencyInjection;
using System;
using Xunit;

namespace Compose.Tests
{
	public class EmissionTests
	{
		[Fact]
		public void CanGenerateDynamicProxy()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IBlank, Dependency>(); });
			var service = app.CreateProxy<IBlank>();
			service.Should().NotBeNull();
		}

		[Fact]
		public void CanGetPropertyOnDynamicProxy()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IGetProperty, GetProperty>(); });
			GetProperty.Id = Guid.NewGuid().ToString();
			var service = app.CreateProxy<IGetProperty>();
			service.Property.Should().Be(GetProperty.Id);
		}

		[Fact]
		public void CanSetPropertyOnDynamicProxy()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<ISetProperty, SetProperty>(); });
			var service = app.CreateProxy<ISetProperty>();
			var id = Guid.NewGuid().ToString();
			service.Property = id;
			SetProperty.Id.Should().Be(id);
		}

		[Fact]
		public void CanInvokeVoidWithoutArgumentsOnDynamicProxy()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IInvokeWithoutArguments, InvokeWithoutArguments>(); });
			var service = app.CreateProxy<IInvokeWithoutArguments>();
			InvokeWithoutArguments.Invoked = false;
			service.Method();
			InvokeWithoutArguments.Invoked.Should().BeTrue();
		}

		[Fact]
		public void CanInvokeVoidWithArgumentsOnDynamicProxy()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IInvokeWithArguments, InvokeWithArguments>(); });
			var service = app.CreateProxy<IInvokeWithArguments>();
			InvokeWithArguments.Invoked = 0;
			service.Method(1);
			InvokeWithArguments.Invoked.Should().Be(1);
		}

		[Fact]
		public void CanReturnInvocationResultOnDynamicProxy()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IReturnFromInvoke, ReturnFromInvoke>(); });
			var service = app.CreateProxy<IReturnFromInvoke>();
			ReturnFromInvoke.Return = 1;
			service.Method().Should().Be(ReturnFromInvoke.Return);
		}

		[Fact]
		public void CanPassThroughExceptionsOnDynamicProxy()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IThrowException, ThrowException>(); });
			var service = app.CreateProxy<IThrowException>();
			Action act = service.Method;
			act.ShouldThrow<TestException>();
		}

		[Fact]
		public void CanChangeImplementationOnDynamicProxy()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IDependency, Dependency1>(); });
			var service = app.CreateProxy<IDependency>();
			Dependency1.Id = Guid.NewGuid().ToString();
			service.GetId().Should().Be(Dependency1.Id);
			var transition = service as ITransition<IDependency>;
			transition.Should().NotBeNull();
			transition.Change(new Dependency2());
			Dependency2.Id = Guid.NewGuid().ToString();
			service.GetId().Should().Be(Dependency2.Id);
		}

        [Fact]
        public void CanInvokeVoidWithGenericArgumentsOnDynamicProxy()
        {
            var app = new Fake.Application();
            app.UseServices(services => { services.AddTransient<IInvokeWithGenericArguments, InvokeWithGenericArguments>(); });
            var service = app.CreateProxy<IInvokeWithGenericArguments>();
            service.Method(1, 2, 3);
        }

        [Fact]
        public void CanReturnGenericInvocationResultOnDynamicProxy()
        {
            var app = new Fake.Application();
            app.UseServices(services => { services.AddTransient<IReturnGenericFromInvoke, ReturnGenericFromInvoke>(); });
            var service = app.CreateProxy<IReturnGenericFromInvoke>();
            service.Method(true).Should().BeTrue();
            service.Method(1).Should().Be(1);
        }
        
		public interface IBlank { }

		private class Dependency : IBlank { }

		public interface IGetProperty { string Property { get; } }

		private class GetProperty : IGetProperty
		{
			internal static string Id { get; set; }
			public string Property { get { return Id; } }
		}

		public interface ISetProperty { string Property { set; } }

		private class SetProperty : ISetProperty
		{
			internal static string Id { get; set; }
			public string Property { set { Id = value; } }
		}

		public interface IInvokeWithoutArguments { void Method(); }

		private class InvokeWithoutArguments : IInvokeWithoutArguments
		{
			internal static bool Invoked { get; set; }

			public void Method() { Invoked = true; }
		}

		public interface IInvokeWithArguments { void Method(int arg); }

		private class InvokeWithArguments : IInvokeWithArguments
		{
			internal static int Invoked { get; set; }
			public void Method(int arg) { Invoked = arg; }
		}

		public interface IReturnFromInvoke { int Method(); }
		private class ReturnFromInvoke : IReturnFromInvoke
		{
			internal static int Return { get; set; }
			public int Method() { return Return; }
		}

		public interface IThrowException { void Method(); }
		private class ThrowException : IThrowException
		{
			public void Method() { throw new TestException(); }
		}

		public class TestException : Exception { }

		public interface IDependency { string GetId(); }
		private class Dependency1 : IDependency
		{
			internal static string Id { get; set; }
			public string GetId() { return Id; }
		}
		private class Dependency2 : IDependency
		{
			internal static string Id { get; set; }
			public string GetId() { return Id; }
		}

        public interface IInvokeWithGenericArguments
        { void Method<T1, T2, T3>(T1 a, T2 b, T3 c); }

        private class InvokeWithGenericArguments : IInvokeWithGenericArguments
        {
            public void Method<T1, T2, T3>(T1 a, T2 b, T3 c) { }
        }

        public interface IReturnGenericFromInvoke { T Method<T>(T arg); }

        private class ReturnGenericFromInvoke : IReturnGenericFromInvoke
        {
            public T Method<T>(T arg) { return arg; }
        }
    }
}
