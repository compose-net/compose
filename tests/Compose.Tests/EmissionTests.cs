using FluentAssertions;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using Xunit;

namespace Compose.Tests
{
	public class EmissionTests
	{
		#region CanGenerateDynamicProxy
		public interface IBlank { }
		private class Dependency : IBlank { }
		[Fact]
		public void CanGenerateDynamicProxy()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IBlank, Dependency>(); });
			var service = app.CreateProxy<IBlank>();
			service.Should().NotBeNull();
		}
		#endregion

		#region CanGetProperty
		public interface IGetProperty { string Property { get; } }
		private class GetProperty : IGetProperty
		{
			internal static string Id { get; set; }
			public string Property { get { return Id; } }
		}
		[Fact]
		public void CanGetProperty()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IGetProperty, GetProperty>(); });
			GetProperty.Id = Guid.NewGuid().ToString();
			var service = app.CreateProxy<IGetProperty>();
			service.Property.Should().Be(GetProperty.Id);
		}
		#endregion

		#region CanSetProperty
		public interface ISetProperty { string Property { set; } }
		private class SetProperty : ISetProperty
		{
			internal static string Id { get; set; }
			public string Property { set { Id = value; } }
		}
		[Fact]
		public void CanSetProperty()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<ISetProperty, SetProperty>(); });
			var service = app.CreateProxy<ISetProperty>();
			var id = Guid.NewGuid().ToString();
			service.Property = id;
			SetProperty.Id.Should().Be(id);
		}
		#endregion

		#region CanInvokeVoidWithoutArguments
		public interface IInvokeWithoutArguments { void Method(); }
		private class InvokeWithoutArguments : IInvokeWithoutArguments
		{
			internal static bool Invoked { get; set; }

			public void Method() { Invoked = true; }
		}
		[Fact]
		public void CanInvokeVoidWithoutArguments()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IInvokeWithoutArguments, InvokeWithoutArguments>(); });
			var service = app.CreateProxy<IInvokeWithoutArguments>();
			InvokeWithoutArguments.Invoked = false;
			service.Method();
			InvokeWithoutArguments.Invoked.Should().BeTrue();
		}
		#endregion

		#region CanInvokeVoidWithArguments
		public interface IInvokeWithArguments { void Method(int arg); }
		private class InvokeWithArguments : IInvokeWithArguments
		{
			internal static int Invoked { get; set; }
			public void Method(int arg) { Invoked = arg; }
		}
		[Fact]
		public void CanInvokeVoidWithArguments()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IInvokeWithArguments, InvokeWithArguments>(); });
			var service = app.CreateProxy<IInvokeWithArguments>();
			InvokeWithArguments.Invoked = 0;
			service.Method(1);
			InvokeWithArguments.Invoked.Should().Be(1);
		}
		#endregion

		#region CanReturnInvocationResult
		public interface IReturnFromInvoke { int Method(); }
		private class ReturnFromInvoke : IReturnFromInvoke
		{
			internal static int Return { get; set; }
			public int Method() { return Return; }
		}
		[Fact]
		public void CanReturnInvocationResult()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IReturnFromInvoke, ReturnFromInvoke>(); });
			var service = app.CreateProxy<IReturnFromInvoke>();
			ReturnFromInvoke.Return = 1;
			service.Method().Should().Be(ReturnFromInvoke.Return);
		}
		#endregion

		#region CanPassThroughExceptions
		public interface IThrowException { void Method(); }
		public class TestException : Exception { }
		private class ThrowException : IThrowException
		{
			public void Method() { throw new TestException(); }
		}
		[Fact]
		public void CanPassThroughExceptions()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IThrowException, ThrowException>(); });
			var service = app.CreateProxy<IThrowException>();
			Action act = service.Method;
			act.ShouldThrow<TestException>();
		}
		#endregion

		#region CanChangeImplementation
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
		[Fact]
		public void CanChangeImplementation()
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
		#endregion

		#region CanInvokeVoidWithGenericArguments
		public interface IInvokeWithGenericArguments { void Method<T1, T2, T3>(T1 a, T2 b, T3 c); }
		private class InvokeWithGenericArguments : IInvokeWithGenericArguments
		{
			public void Method<T1, T2, T3>(T1 a, T2 b, T3 c) { }
		}
		[Fact]
		public void CanInvokeVoidWithGenericArguments()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IInvokeWithGenericArguments, InvokeWithGenericArguments>(); });
			var service = app.CreateProxy<IInvokeWithGenericArguments>();
			service.Method(1, 2, 3);
		}
		#endregion

		#region CanReturnGenericInvocationResult
		public interface IReturnGenericFromInvoke { T Method<T>(T arg); }

		private class ReturnGenericFromInvoke : IReturnGenericFromInvoke
		{
			public T Method<T>(T arg) { return arg; }
		}
		[Fact]
		public void CanReturnGenericInvocationResult()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IReturnGenericFromInvoke, ReturnGenericFromInvoke>(); });
			var service = app.CreateProxy<IReturnGenericFromInvoke>();
			service.Method(true).Should().BeTrue();
			service.Method(1).Should().Be(1);
		}
		#endregion

		#region CanInvokeVoidWithInterfaceConstrainedGenericArguments
		public interface IInvokeWithInterfaceConstrainedGenericArguments { void Method<T>(T arg) where T : IDependency; }

		private class InvokeWithInterfaceConstrainedGenericArguments : IInvokeWithInterfaceConstrainedGenericArguments
		{
			public void Method<T>(T arg) where T : IDependency { }
		}
		[Fact]
		public void CanInvokeVoidWithInterfaceConstrainedGenericArguments()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IInvokeWithInterfaceConstrainedGenericArguments, InvokeWithInterfaceConstrainedGenericArguments>(); });
			var service = app.CreateProxy<IInvokeWithInterfaceConstrainedGenericArguments>();
			service.Method(new Dependency1());
		}
		#endregion

		#region CanInvokeVoidWithBaseClassConstrainedGenericArguments
		public interface IInvokeWithBaseClassConstrainedGenericArguments { void Method<T>(T arg) where T : Base; }
		private class InvokeWithBaseClassConstrainedGenericArguments : IInvokeWithBaseClassConstrainedGenericArguments
		{
			public void Method<T>(T arg) where T : Base { }
		}
		[Fact]
		public void CanInvokeVoidWithBaseClassConstrainedGenericArguments()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IInvokeWithBaseClassConstrainedGenericArguments, InvokeWithBaseClassConstrainedGenericArguments>(); });
			var service = app.CreateProxy<IInvokeWithBaseClassConstrainedGenericArguments>();
			service.Method(new Derivative());
		}
		#endregion

		#region CanInvokeWithDefaultConstructorConstrainedGenericArgument
		public interface IInvokeWithDefaultConstructorConstrainedGenericArguments { void Method<T>(T arg) where T : new(); }
		private class InvokeWithDefaultConstructorConstrainedGenericArguments : IInvokeWithDefaultConstructorConstrainedGenericArguments
		{
			public void Method<T>(T arg) where T : new() { }
		}
		[Fact]
		public void CanInvokeWithDefaultConstructorConstrainedGenericArgument()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IInvokeWithDefaultConstructorConstrainedGenericArguments, InvokeWithDefaultConstructorConstrainedGenericArguments>(); });
			var service = app.CreateProxy<IInvokeWithDefaultConstructorConstrainedGenericArguments>();
			service.Method(new object());
		}
		#endregion

		#region CanInvokeWithClassConstrainedGenericArgument
		public interface IInvokeWithClassConstrainedGenericArguments { void Method<T>(T arg) where T : class; }
		private class InvokeWithClassConstrainedGenericArguments : IInvokeWithClassConstrainedGenericArguments
		{
			public void Method<T>(T arg) where T : class { }
		}
		[Fact]
		public void CanInvokeWithClassConstrainedGenericArgument()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IInvokeWithClassConstrainedGenericArguments, InvokeWithClassConstrainedGenericArguments>(); });
			var service = app.CreateProxy<IInvokeWithClassConstrainedGenericArguments>();
			service.Method(new object());
		}
		#endregion

		#region CanInvokeWithStructConstrainedGenericArgument
		public interface IInvokeWithStructConstrainedGenericArguments { void Method<T>(T arg) where T : struct; }
		private class InvokeWithStructConstrainedGenericArguments : IInvokeWithStructConstrainedGenericArguments
		{
			public void Method<T>(T arg) where T : struct { }
		}
		[Fact]
		public void CanInvokeWithStructConstrainedGenericArgument()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IInvokeWithStructConstrainedGenericArguments, InvokeWithStructConstrainedGenericArguments>(); });
			var service = app.CreateProxy<IInvokeWithStructConstrainedGenericArguments>();
			service.Method(1);
		}
		#endregion

		#region CanInvokeOverloadedMethods
		public interface IInvokeOverloadedMethods
		{
			void Method(int arg);
			void Method(string arg);
		}
		private class InvokeOverloadedMethods : IInvokeOverloadedMethods
		{
			public void Method(int arg) { }
			public void Method(string arg) { }
		}
		[Fact]
		public void CanInvokeOverloadedMethods()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IInvokeOverloadedMethods, InvokeOverloadedMethods>(); });
			var service = app.CreateProxy<IInvokeOverloadedMethods>();
			service.Method(1);
			service.Method("a");
		}
		#endregion

		#region CanInvokeWithNestedGenericArgument
		public interface IInvokeWithNestedGenericArgument { void Method<T>(List<List<T>> arg); }
		private class InvokeWithNestedGenericArgument : IInvokeWithNestedGenericArgument
		{
			public void Method<T>(List<List<T>> arg) { }
		}
		[Fact]
		public void CanInvokeWithNestedGenericArgument()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IInvokeWithNestedGenericArgument, InvokeWithNestedGenericArgument>(); });
			var service = app.CreateProxy<IInvokeWithNestedGenericArgument>();
			service.Method(new List<List<int>>());
		}
		#endregion

		#region CanInvokeWithClassDefinedGenericArgument
		public interface IInvokeWithClassDefinedGenericArgument<T> { void Method(T arg); }
		private class InvokeWithClassDefinedGenericArgument<T> : IInvokeWithClassDefinedGenericArgument<T>
		{
			public void Method(T arg) { }
		}
		[Fact]
		public void CanInvokeWithClassDefinedGenericArgument()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IInvokeWithClassDefinedGenericArgument<int>, InvokeWithClassDefinedGenericArgument<int>>(); });
			var service = app.CreateProxy<IInvokeWithClassDefinedGenericArgument<int>>();
			service.Method(1);
		}
		#endregion

		#region CanInvokeWithClassGenericConstrainedGenericArguments
		public interface IInvokeWithClassGenericConstrainedGenericArguments<TBase> { void Method<TDerivative>(TDerivative derivative) where TDerivative : TBase; }
		private class InvokeWithClassGenericConstrainedGenericArguments<TBase> : IInvokeWithClassGenericConstrainedGenericArguments<TBase>
		{
			public void Method<TDerivative>(TDerivative derivative) where TDerivative : TBase { }
		}
		[Fact]
		public void CanInvokeWithClassGenericConstrainedGenericArguments()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IInvokeWithClassGenericConstrainedGenericArguments<Base>, InvokeWithClassGenericConstrainedGenericArguments<Base>>(); });
			var service = app.CreateProxy<IInvokeWithClassGenericConstrainedGenericArguments<Base>>();
			service.Method(new Derivative());
		}
		#endregion

		#region CanInvokeWithMethodGenericConstrainedGenericArguments
		public interface IInvokeWithMethodGenericConstrainedGenericArguments { void Method<TBase, TDerivative>(TBase arg1, TDerivative arg2) where TDerivative : TBase; }
		private class InvokeWithMethodGenericConstrainedGenericArguments : IInvokeWithMethodGenericConstrainedGenericArguments
		{
			public void Method<TBase, TDerivative>(TBase arg1, TDerivative arg2) where TDerivative : TBase { }
		}
		[Fact]
		public void CanInvokeWithMethodGenericConstrainedGenericArguments()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IInvokeWithMethodGenericConstrainedGenericArguments, InvokeWithMethodGenericConstrainedGenericArguments>(); });
			var service = app.CreateProxy<IInvokeWithMethodGenericConstrainedGenericArguments>();
			service.Method((Base)null, new Derivative());
		}
		#endregion

		#region CanInvokeWithByRefArguments
		public interface IInvokeWithByRefArgument { void Method(ref int arg); }
		private class InvokeWithByRefArgument : IInvokeWithByRefArgument
		{
			public void Method(ref int arg) { }
		}
		[Fact]
		public void CanInvokeWithByRefArguments()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IInvokeWithByRefArgument, InvokeWithByRefArgument>(); });
			var service = app.CreateProxy<IInvokeWithByRefArgument>();
			var arg = 0;
			service.Method(ref arg);
        }
		#endregion

		#region CanInvokeWithOutArguments
		public interface IInvokeWithOutArguments { void Method(out int arg); }
		private class InvokeWithOutArguments : IInvokeWithOutArguments
		{
			public void Method(out int arg) { arg = 4; }
		}
		[Fact]
		public void CanInvokeWithOutArguments()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IInvokeWithOutArguments, InvokeWithOutArguments>(); });
			var service = app.CreateProxy<IInvokeWithOutArguments>();
			var arg = 0;
			service.Method(out arg);
        }
		#endregion

		#region CanInvokeWithParamsArguments
		public interface IInvokeWithParamsArguments { void Method<T>(params T[] arg); }
		private class InvokeWithParamsArguments : IInvokeWithParamsArguments
		{
			public void Method<T>(params T[] arg) { }
		}
		[Fact]
		public void CanInvokeWithParamsArguments()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IInvokeWithParamsArguments, InvokeWithParamsArguments>(); });
			var service = app.CreateProxy<IInvokeWithParamsArguments>();
			service.Method(1, 2, 3);
        }
		#endregion

		#region CanInvokeWithAllGenericConstraints
		public interface IInvokeWithAllGenericConstraints<TClass>
		{
			void Method<TMethodBase, TMethodDerivative>(ref TMethodBase arg1, out TMethodDerivative arg2, params TMethodDerivative[] arg3)
				where TMethodBase : TClass where TMethodDerivative : class, TMethodBase, IDisposable, new();
		}
		private class InvokeWithAllGenericConstraints<TClass> : IInvokeWithAllGenericConstraints<TClass>
		{
			public void Method<TMethodBase, TMethodDerivative>(ref TMethodBase arg1, out TMethodDerivative arg2, params TMethodDerivative[] arg3)
				where TMethodBase : TClass where TMethodDerivative : class, TMethodBase, IDisposable, new()
			{ arg2 = new TMethodDerivative(); }
		}
		private class LowerDerivative : Derivative, IDisposable { public void Dispose() { } }
		[Fact]
		public void CanInvokeWithAllGenericConstraints()
		{
			var app = new Fake.Application();
			app.UseServices(services => { services.AddTransient<IInvokeWithAllGenericConstraints<Base>, InvokeWithAllGenericConstraints<Base>>(); });
			var service = app.CreateProxy<IInvokeWithAllGenericConstraints<Base>>();
			var arg1 = new Derivative();
			var arg2 = new LowerDerivative();
			service.Method(ref arg1, out arg2);
        }
		#endregion

		#region CanInvokeInheritedInterfaceMethods
		public interface INestedInterface { void NestedMethod(); }
		public interface IParentInterface : INestedInterface { void ParentMethod(); }
		private class InvokeInheritedInterfaceMethods : IParentInterface
		{
			public void NestedMethod() { }

			public void ParentMethod() { }
		}
        [Fact]
		public void CanInvokeInheritedInterfaceMethods()
		{
			var app = new Fake.Application();
			app.UseServices(services => services.AddTransient<IParentInterface, InvokeInheritedInterfaceMethods>());
			var service = app.CreateProxy<IParentInterface>();
			service.ParentMethod();
			service.NestedMethod();
		}
		#endregion

		#region CanInvokeInheritedInterfaceExplcitlyImplementedMethods
		public interface IExplicitInterface1 { void Method(); }
		public interface IExplicitInterface2 { void Method(); }
		public interface IInvokeInheritedInterfaceExplcitlyImplementedMethods : IExplicitInterface1, IExplicitInterface2 { }
        private class InvokeInheritedInterfaceExplcitlyImplementedMethods : IInvokeInheritedInterfaceExplcitlyImplementedMethods
		{
			void IExplicitInterface1.Method() { }
			void IExplicitInterface2.Method() { }
		}
		[Fact]
		public void CanInvokeInheritedInterfaceExplcitlyImplementedMethods()
		{
			var app = new Fake.Application();
			app.UseServices(services => services.AddTransient<IInvokeInheritedInterfaceExplcitlyImplementedMethods, InvokeInheritedInterfaceExplcitlyImplementedMethods>());
			var service = app.CreateProxy<IInvokeInheritedInterfaceExplcitlyImplementedMethods>();
			((IExplicitInterface1)service).Method();
			((IExplicitInterface2)service).Method();
		}
		#endregion

		#region CanGenerateProxyForOptions
		private class Options { }
		private class OptionsImplementation<T> : IOptions<T> where T : class, new()
		{
			public T Options { get; }

			public T GetNamedOptions(string name) { return null; }
		}
		[Fact]
		public void CanGenerateProxyForOptions()
		{
			var app = new Fake.Application();
			app.UseServices(services => services.AddSingleton(typeof(IOptions<>), typeof(OptionsImplementation<>)));
			Action act = () => app.CreateProxy<IOptions<Options>>();
			act.ShouldNotThrow<UnsupportedClassDefintionException>();
		}
		#endregion

		#region Common Classes
		public abstract class Base { }

		private class Derivative : Base { }
		#endregion	
	}
}
