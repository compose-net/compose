﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using TestAttributes;
using Xunit;

namespace Compose.Tests
{
	public class EmissionTests
	{
		#region CanGenerateDynamicProxy
		public interface IBlank { }
		internal class Dependency : IBlank { }
		[Fact]
		public void CanGenerateDynamicProxy()
		{
			Assert.NotNull(SetupProxy<IBlank, Dependency>());
		}
		#endregion

		#region CanGetProperty
		public interface IGetProperty { string Property { get; } }
		internal class GetProperty : IGetProperty
		{
			internal static string Id { get; set; }
			public string Property { get { return Id; } }
		}
		[Fact]
		public void CanGetProperty()
		{
			Assert.Equal(GetProperty.Id, SetupProxy<IGetProperty, GetProperty>()().Property);
		}
		#endregion

		#region CanSetProperty
		public interface ISetProperty { string Property { set; } }
		internal class SetProperty : ISetProperty
		{
			internal static string Id { get; set; }
			public string Property { set { Id = value; } }
		}
		[Fact]
		public void CanSetProperty()
		{
			var service = SetupProxy<ISetProperty, SetProperty>()();
			var id = Guid.NewGuid().ToString();
			service.Property = id;
			Assert.Equal(id, SetProperty.Id);
		}
		#endregion

		#region CanInvokeVoidWithoutArguments
		public interface IInvokeWithoutArguments { void Method(); }
		internal class InvokeWithoutArguments : IInvokeWithoutArguments
		{
			internal static bool Invoked { get; set; }

			public void Method() { Invoked = true; }
		}
		[Fact]
		public void CanInvokeVoidWithoutArguments()
		{
			var service = SetupProxy<IInvokeWithoutArguments, InvokeWithoutArguments>()();
			InvokeWithoutArguments.Invoked = false;
			service.Method();
			Assert.True(InvokeWithoutArguments.Invoked);
		}
		#endregion

		#region CanInvokeVoidWithArguments
		public interface IInvokeWithArguments { void Method(int arg); }
		internal class InvokeWithArguments : IInvokeWithArguments
		{
			internal static int Invoked { get; set; }
			public void Method(int arg) { Invoked = arg; }
		}
		[Fact]
		public void CanInvokeVoidWithArguments()
		{
			var service = SetupProxy<IInvokeWithArguments, InvokeWithArguments>()();
			InvokeWithArguments.Invoked = 0;
			service.Method(1);
			Assert.Equal(1, InvokeWithArguments.Invoked);
		}
		#endregion

		#region CanReturnInvocationResult
		public interface IReturnFromInvoke { int Method(); }
		internal class ReturnFromInvoke : IReturnFromInvoke
		{
			internal static int Return { get; set; }
			public int Method() { return Return; }
		}
		[Fact]
		public void CanReturnInvocationResult()
		{
			var service = SetupProxy<IReturnFromInvoke, ReturnFromInvoke>()();
			ReturnFromInvoke.Return = 1;
			Assert.Equal(ReturnFromInvoke.Return, service.Method());
		}
		#endregion

		#region CanPassThroughExceptions
		public interface IThrowException { void Method(); }
		public class TestException : Exception { }
		internal class ThrowException : IThrowException
		{
			public void Method() { throw new TestException(); }
		}
		[Fact]
		public void CanPassThroughExceptions()
		{
			Action act = () => SetupProxy<IThrowException, ThrowException>()().Method();
			Assert.IsType(typeof(TestException), Record.Exception(act));
		}
		#endregion

		#region CanChangeImplementation
		public interface IDependency { string GetId(); }
		internal class Dependency1 : IDependency
		{
			internal static string Id { get; set; }
			public string GetId() { return Id; }
		}
		internal class Dependency2 : IDependency
		{
			internal static string Id { get; set; }
			public string GetId() { return Id; }
		}
		[Fact]
		public void CanChangeImplementation()
		{
			var app = new Fake.Executable();
			app.UseServices(services => services.AddTransitional<IDependency, Dependency1>());
			var service = app.ApplicationServices.GetRequiredService<IDependency>();
			Dependency1.Id = Guid.NewGuid().ToString();
			Assert.Equal(Dependency1.Id, service.GetId());
			var transition = app.ApplicationServices.GetRequiredService<ITransitionManager<IDependency>>();
			transition.Change(() => new Dependency2());
			Dependency2.Id = Guid.NewGuid().ToString();
			Assert.Equal(Dependency2.Id, service.GetId());
		}
		#endregion

		#region CanInvokeVoidWithGenericArguments
		public interface IInvokeWithGenericArguments { void Method<T1, T2, T3>(T1 a, T2 b, T3 c); }
		internal class InvokeWithGenericArguments : IInvokeWithGenericArguments
		{
			public void Method<T1, T2, T3>(T1 a, T2 b, T3 c) { }
		}
		[Fact]
		public void CanInvokeVoidWithGenericArguments()
		{
			var service = SetupProxy<IInvokeWithGenericArguments, InvokeWithGenericArguments>()();
			service.Method(1, 2, 3);
		}
		#endregion

		#region CanReturnGenericInvocationResult
		public interface IReturnGenericFromInvoke { T Method<T>(T arg); }

		internal class ReturnGenericFromInvoke : IReturnGenericFromInvoke
		{
			public T Method<T>(T arg) { return arg; }
		}
		[Fact]
		public void CanReturnGenericInvocationResult()
		{
			var service = SetupProxy<IReturnGenericFromInvoke, ReturnGenericFromInvoke>()();
			Assert.True(service.Method(true));
			Assert.Equal(1, service.Method(1));
		}
		#endregion

		#region CanGenerateProxyWithInterfaceConstrainedGenericArguments
		public interface IInterfaceConstraint { }
		public interface IInterfaceConstrainedGenericArgument<T> where T : IInterfaceConstraint { }
		internal class InterfaceConstrainedGenericArgument<T> : IInterfaceConstrainedGenericArgument<T> where T : IInterfaceConstraint { }
		[Fact]
		public void CanGenerateProxyWithInterfaceConstrainedGenericArguments()
		{
			Assert.NotNull(Record.Exception(CreateProxy(typeof(IInterfaceConstrainedGenericArgument<>), typeof(InterfaceConstrainedGenericArgument<>))));
		}
		#endregion

		#region CanGenerateProxyWithBaseClassConstrainedGenericArguments
		public interface IInterfaceWithBaseClassConstrainedGenericArgument<T> where T : Base { }
		internal class InterfaceWithBaseClassConstrainedGenericArgument<T> : IInterfaceWithBaseClassConstrainedGenericArgument<T> where T : Derivative { }
		[Fact]
		public void CanGenerateProxyWithBaseClassConstrainedGenericArguments()
		{
			Assert.NotNull(Record.Exception(CreateProxy(typeof(IInterfaceWithBaseClassConstrainedGenericArgument<>), typeof(InterfaceWithBaseClassConstrainedGenericArgument<>))));
        }
		#endregion

		#region CanGenerateProxyWithDefaultConstructorConstrainedGenericArguments
		public interface IInterfaceWithDefaultConstructorConstrainedGenericArgument<T> where T : new() { }
		internal class InterfaceWithDefaultConstructorConstrainedGenericArgument<T> : IInterfaceWithDefaultConstructorConstrainedGenericArgument<T> where T : new() { }
		[Fact]
		public void CanGenerateProxyWithDefaultConstructorConstrainedGenericArguments()
		{
			Assert.NotNull(Record.Exception(CreateProxy(typeof(IInterfaceWithDefaultConstructorConstrainedGenericArgument<>), typeof(InterfaceWithDefaultConstructorConstrainedGenericArgument<>))));
        }
		#endregion

		#region CanGenerateProxyWithClassConstrainedGenericArguments
		public interface IInterfaceWithClassConstrainedGenericArgument<T> where T : class { }
		internal class InterfaceWithClassConstrainedGenericArgument<T> : IInterfaceWithClassConstrainedGenericArgument<T> where T : class { }
        [Fact]
		public void CanGenerateProxyWithClassConstrainedGenericArguments()
		{
			Assert.NotNull(Record.Exception(CreateProxy(typeof(IInterfaceWithClassConstrainedGenericArgument<>), typeof(InterfaceWithClassConstrainedGenericArgument<>))));
        }
		#endregion

		#region CanGenerateProxyWithStructConstrainedGenericArguments
		public interface IInterfaceWithStructConstrainedGenericArgument<T> where T : struct { }
		internal class InterfaceWithStructConstrainedGenericArgument<T> : IInterfaceWithStructConstrainedGenericArgument<T> where T : struct { }
		[Fact]
		public void CanGenerateProxyWithStructConstrainedGenericArguments()
		{
			Assert.NotNull(Record.Exception(CreateProxy(typeof(IInterfaceWithStructConstrainedGenericArgument<>), typeof(InterfaceWithStructConstrainedGenericArgument<>))));
        }
		#endregion

		#region CanGenerateProxyWithCovariantConstrainedGenericsArguments
		public interface IInterfaceWithCovariantConstrainedGenericArguments<in TIn, out TOut> { }
		internal class InterfaceWithCovariantConstrainedGenericArguments<TIn, TOut> : IInterfaceWithCovariantConstrainedGenericArguments<TIn, TOut> { }
		[Fact]
		public void CanGenerateProxyWithCovariantConstrainedGenericsArguments()
		{
			Assert.NotNull(Record.Exception(CreateProxy(typeof(IInterfaceWithCovariantConstrainedGenericArguments<,>), typeof(InterfaceWithCovariantConstrainedGenericArguments<,>))));
        }
		#endregion

		#region CanInvokeVoidWithInterfaceConstrainedGenericArguments
		public interface IInvokeWithInterfaceConstrainedGenericArguments { void Method<T>(T arg) where T : IDependency; }

		internal class InvokeWithInterfaceConstrainedGenericArguments : IInvokeWithInterfaceConstrainedGenericArguments
		{
			public void Method<T>(T arg) where T : IDependency { }
		}
		[Fact]
		public void CanInvokeVoidWithInterfaceConstrainedGenericArguments()
		{
			var service = SetupProxy<IInvokeWithInterfaceConstrainedGenericArguments, InvokeWithInterfaceConstrainedGenericArguments>()();
			service.Method(new Dependency1());
		}
		#endregion

		#region CanInvokeVoidWithBaseClassConstrainedGenericArguments
		public interface IInvokeWithBaseClassConstrainedGenericArguments { void Method<T>(T arg) where T : Base; }
		internal class InvokeWithBaseClassConstrainedGenericArguments : IInvokeWithBaseClassConstrainedGenericArguments
		{
			public void Method<T>(T arg) where T : Base { }
		}
		[Fact]
		public void CanInvokeVoidWithBaseClassConstrainedGenericArguments()
		{
			var service = SetupProxy<IInvokeWithBaseClassConstrainedGenericArguments, InvokeWithBaseClassConstrainedGenericArguments>()();
			service.Method(new Derivative());
		}
		#endregion

		#region CanInvokeWithDefaultConstructorConstrainedGenericArgument
		public interface IInvokeWithDefaultConstructorConstrainedGenericArguments { void Method<T>(T arg) where T : new(); }
		internal class InvokeWithDefaultConstructorConstrainedGenericArguments : IInvokeWithDefaultConstructorConstrainedGenericArguments
		{
			public void Method<T>(T arg) where T : new() { }
		}
		[Fact]
		public void CanInvokeWithDefaultConstructorConstrainedGenericArgument()
		{
			var service = SetupProxy<IInvokeWithDefaultConstructorConstrainedGenericArguments, InvokeWithDefaultConstructorConstrainedGenericArguments>()();
			service.Method(new object());
		}
		#endregion

		#region CanInvokeWithClassConstrainedGenericArgument
		public interface IInvokeWithClassConstrainedGenericArguments { void Method<T>(T arg) where T : class; }
		internal class InvokeWithClassConstrainedGenericArguments : IInvokeWithClassConstrainedGenericArguments
		{
			public void Method<T>(T arg) where T : class { }
		}
		[Fact]
		public void CanInvokeWithClassConstrainedGenericArgument()
		{
			var service = SetupProxy<IInvokeWithClassConstrainedGenericArguments, InvokeWithClassConstrainedGenericArguments>()();
			service.Method(new object());
		}
		#endregion

		#region CanInvokeWithStructConstrainedGenericArgument
		public interface IInvokeWithStructConstrainedGenericArguments { void Method<T>(T arg) where T : struct; }
		internal class InvokeWithStructConstrainedGenericArguments : IInvokeWithStructConstrainedGenericArguments
		{
			public void Method<T>(T arg) where T : struct { }
		}
		[Fact]
		public void CanInvokeWithStructConstrainedGenericArgument()
		{
			var service = SetupProxy<IInvokeWithStructConstrainedGenericArguments, InvokeWithStructConstrainedGenericArguments>()();
			service.Method(1);
		}
		#endregion

		#region CanInvokeWithClassGenericConstrainedGenericArguments
		public interface IInvokeWithClassGenericConstrainedGenericArguments<TBase> { void Method<TDerivative>(TDerivative derivative) where TDerivative : TBase; }
		internal class InvokeWithClassGenericConstrainedGenericArguments<TBase> : IInvokeWithClassGenericConstrainedGenericArguments<TBase>
		{
			public void Method<TDerivative>(TDerivative derivative) where TDerivative : TBase { }
		}
		[Fact]
		public void CanInvokeWithClassGenericConstrainedGenericArguments()
		{
			var service = SetupProxy<IInvokeWithClassGenericConstrainedGenericArguments<Base>, InvokeWithClassGenericConstrainedGenericArguments<Base>>()();
			service.Method(new Derivative());
		}
		#endregion

		#region CanInvokeWithMethodGenericConstrainedGenericArguments
		public interface IInvokeWithMethodGenericConstrainedGenericArguments { void Method<TBase, TDerivative>(TBase arg1, TDerivative arg2) where TDerivative : TBase; }
		internal class InvokeWithMethodGenericConstrainedGenericArguments : IInvokeWithMethodGenericConstrainedGenericArguments
		{
			public void Method<TBase, TDerivative>(TBase arg1, TDerivative arg2) where TDerivative : TBase { }
		}
		[Fact]
		public void CanInvokeWithMethodGenericConstrainedGenericArguments()
		{
			var service = SetupProxy<IInvokeWithMethodGenericConstrainedGenericArguments, InvokeWithMethodGenericConstrainedGenericArguments>()();
			service.Method((Base)null, new Derivative());
		}
		#endregion

		#region CanInvokeWithAllGenericConstraints
		public interface IInvokeWithAllGenericConstraints<TClass>
		{
			void Method<TMethodBase, TMethodDerivative>(ref TMethodBase arg1, out TMethodDerivative arg2, params TMethodDerivative[] arg3)
				where TMethodBase : TClass where TMethodDerivative : class, TMethodBase, IDisposable, new();
		}
		internal class InvokeWithAllGenericConstraints<TClass> : IInvokeWithAllGenericConstraints<TClass>
		{
			public void Method<TMethodBase, TMethodDerivative>(ref TMethodBase arg1, out TMethodDerivative arg2, params TMethodDerivative[] arg3)
				where TMethodBase : TClass where TMethodDerivative : class, TMethodBase, IDisposable, new()
			{ arg2 = new TMethodDerivative(); }
		}
		internal class LowerDerivative : Derivative, IDisposable { public void Dispose() { } }
		[Fact]
		public void CanInvokeWithAllGenericConstraints()
		{
			var service = SetupProxy<IInvokeWithAllGenericConstraints<Base>, InvokeWithAllGenericConstraints<Base>>()();
			var arg1 = new Derivative();
			var arg2 = new LowerDerivative();
			service.Method(ref arg1, out arg2);
		}
		#endregion

		#region CanInvokeOverloadedMethods
		public interface IInvokeOverloadedMethods
		{
			void Method(int arg);
			void Method(string arg);
		}
		internal class InvokeOverloadedMethods : IInvokeOverloadedMethods
		{
			public void Method(int arg) { }
			public void Method(string arg) { }
		}
		[Fact]
		public void CanInvokeOverloadedMethods()
		{
			var service = SetupProxy<IInvokeOverloadedMethods, InvokeOverloadedMethods>()();
			service.Method(1);
			service.Method("a");
		}
		#endregion

		#region CanInvokeWithNestedGenericArgument
		public interface IInvokeWithNestedGenericArgument { void Method<T>(List<List<T>> arg); }
		internal class InvokeWithNestedGenericArgument : IInvokeWithNestedGenericArgument
		{
			public void Method<T>(List<List<T>> arg) { }
		}
		[Fact]
		public void CanInvokeWithNestedGenericArgument()
		{
			var service = SetupProxy<IInvokeWithNestedGenericArgument, InvokeWithNestedGenericArgument>()();
			service.Method(new List<List<int>>());
		}
		#endregion

		#region CanInvokeWithClassDefinedGenericArgument
		public interface IInvokeWithClassDefinedGenericArgument<T> { void Method(T arg); }
		internal class InvokeWithClassDefinedGenericArgument<T> : IInvokeWithClassDefinedGenericArgument<T>
		{
			public void Method(T arg) { }
		}
		[Fact]
		public void CanInvokeWithClassDefinedGenericArgument()
		{
			var service = SetupProxy<IInvokeWithClassDefinedGenericArgument<int>, InvokeWithClassDefinedGenericArgument<int>>()();
			service.Method(1);
		}
		#endregion

		#region CanInvokeWithByRefArguments
		public interface IInvokeWithByRefArgument { void Method(ref int arg); }
		internal class InvokeWithByRefArgument : IInvokeWithByRefArgument
		{
			public void Method(ref int arg) { }
		}
		[Fact]
		public void CanInvokeWithByRefArguments()
		{
			var service = SetupProxy<IInvokeWithByRefArgument, InvokeWithByRefArgument>()();
			var arg = 0;
			service.Method(ref arg);
		}
		#endregion

		#region CanInvokeWithOutArguments
		public interface IInvokeWithOutArguments { void Method(out int arg); }
		internal class InvokeWithOutArguments : IInvokeWithOutArguments
		{
			public void Method(out int arg) { arg = 4; }
		}
		[Fact]
		public void CanInvokeWithOutArguments()
		{
			var service = SetupProxy<IInvokeWithOutArguments, InvokeWithOutArguments>()();
			var arg = 0;
			service.Method(out arg);
		}
		#endregion

		#region CanInvokeWithParamsArguments
		public interface IInvokeWithParamsArguments { void Method<T>(params T[] arg); }
		internal class InvokeWithParamsArguments : IInvokeWithParamsArguments
		{
			public void Method<T>(params T[] arg) { }
		}
		[Fact]
		public void CanInvokeWithParamsArguments()
		{
			var service = SetupProxy<IInvokeWithParamsArguments, InvokeWithParamsArguments>()();
			service.Method(1, 2, 3);
		}
		#endregion

		#region CanInvokeInheritedInterfaceMethods
		public interface INestedInterface { void NestedMethod(); }
		public interface IParentInterface : INestedInterface { void ParentMethod(); }
		internal class InvokeInheritedInterfaceMethods : IParentInterface
		{
			public void NestedMethod() { }

			public void ParentMethod() { }
		}
		[Fact]
		public void CanInvokeInheritedInterfaceMethods()
		{
			var service = SetupProxy<IParentInterface, InvokeInheritedInterfaceMethods>()();
			service.ParentMethod();
			service.NestedMethod();
		}
		#endregion

		#region CanInvokeInheritedInterfaceExplcitlyImplementedMethods
		public interface IExplicitInterface1 { void Method(); }
		public interface IExplicitInterface2 { void Method(); }
		public interface IInvokeInheritedInterfaceExplcitlyImplementedMethods : IExplicitInterface1, IExplicitInterface2 { }
		internal class InvokeInheritedInterfaceExplcitlyImplementedMethods : IInvokeInheritedInterfaceExplcitlyImplementedMethods
		{
			void IExplicitInterface1.Method() { }
			void IExplicitInterface2.Method() { }
		}
		[Fact]
		public void CanInvokeInheritedInterfaceExplcitlyImplementedMethods()
		{
			var service = SetupProxy<IInvokeInheritedInterfaceExplcitlyImplementedMethods, InvokeInheritedInterfaceExplcitlyImplementedMethods>()();
			((IExplicitInterface1)service).Method();
			((IExplicitInterface2)service).Method();
		}
		#endregion

		#region CanGenerateCovariantProxies
		public interface ICovariant<out T> { T Method(); }
		internal class Covariant : ICovariant<string>
		{
			public string Method() { return null; }
		}
		[Fact]
		public void CanGenerateCovariantProxies()
		{
            Assert.Null(Record.Exception(InvokeProxy<ICovariant<string>, Covariant>()));
		}
		#endregion

		#region CanGenerateProxyForSystemInterfaces
		internal class Disposable : IDisposable
		{
			public void Dispose() { }
		}
		[Fact]
		public void CanGenerateProxyForSystemInterfaces()
		{
			Assert.Null(Record.Exception(InvokeProxy<IDisposable, Disposable>()));
		}
		#endregion

		#region CanGenerateProxyForUntypedGenerics
		public interface IUntypedGeneric<T> { void Method(); }
		internal class UntypedGeneric<T> : IUntypedGeneric<T>
		{
			public void Method() { }
		}
		[Fact]
		public void CanGenerateProxyForUntypedGenerics()
		{
			Assert.Null(Record.Exception(InvokeProxy<IUntypedGeneric<string>>(typeof(IUntypedGeneric<>), typeof(UntypedGeneric<>))));
		}
		#endregion

		#region CanThrowInformativeExceptionWhenInterfaceIsNotVisible
		private interface IInformativeExceptionThrownForInvisibleInterface { }
		internal class InformativeExceptionThrownForInvisibleInterface : IInformativeExceptionThrownForInvisibleInterface { }
		[Fact]
		public void CanThrowInformativeExceptionWhenInterfaceIsNotVisible()
		{

			Assert.IsType(
				typeof(InaccessibleTypeException), 
				Record.Exception(InvokeProxy<IInformativeExceptionThrownForInvisibleInterface, InformativeExceptionThrownForInvisibleInterface>())
			);
		}
		#endregion

		#region CanThrowInformativeExceptionWhenGenericTypeIsNotVisible
		public interface IInformativeExceptionThrownForInvisibleGeneric<T> { }
		private class InformativeExceptionThrownForInvisibleInterfaceGeneric { }
		private class InformativeExceptionThrownForInvisibleGeneric 
			: IInformativeExceptionThrownForInvisibleGeneric<InformativeExceptionThrownForInvisibleInterfaceGeneric> { }
		[Fact]
		public void CanThrowInformativeExceptionWhenGenericTypeIsNotVisible()
		{
			Assert.IsType(
				typeof(InaccessibleTypeException), 
				Record.Exception(InvokeProxy<IInformativeExceptionThrownForInvisibleGeneric<InformativeExceptionThrownForInvisibleInterfaceGeneric>, InformativeExceptionThrownForInvisibleGeneric>())
			);
		}
		#endregion

		#region CanGenerateGenericProxy
		public interface IGeneric<T> { }
		internal class Generic<T> : IGeneric<T> { }
		[Fact]
		public void CanGenerateGenericProxy()
		{
			Assert.NotNull(Record.Exception(CreateProxy(typeof(IGeneric<>), typeof(Generic<>))));
		}
		#endregion

		#region CanResolveGenericProxy
		[Fact]
		public void CanResolveGenericProxy()
		{
			Assert.Null(Record.Exception(InvokeProxy<IGeneric<object>>(typeof(IGeneric<>), typeof(Generic<>))));
		}

		#endregion

		private static Action CreateProxy(Type interfaceType, Type implementationType)
		{
			var app = new Fake.Executable();
			app.UseServices(services => services.AddTransient(interfaceType, implementationType));
			return () => app.CreateProxy(interfaceType.GetTypeInfo());
		}

		private static Action InvokeProxy<TInterface, TImplementation>()
			where TImplementation : TInterface where TInterface : class
		{
			return () => SetupProxy<TInterface>(typeof(TImplementation))();
		}

		private static Action InvokeProxy<T>(Type implementationType) where T : class
		{
			return () => SetupProxy<T>(implementationType)();
		}

		private static Action InvokeProxy<T>(Type interfaceType, Type implementationType) where T : class
		{
			return () => SetupProxy<T>(interfaceType, implementationType);
		}

		private static Func<TInterface> SetupProxy<TInterface, TImplementation>()
			where TImplementation : TInterface where TInterface : class
		{
			return SetupProxy<TInterface>(typeof(TImplementation));
		}

		private static Func<T> SetupProxy<T>(Type implementationType) where T : class
		{
			return SetupProxy<T>(typeof(T), implementationType);
		}

		private static Func<T> SetupProxy<T>(Type interfaceType, Type implementationType) where T : class
		{
			var app = new Fake.Executable();
			app.UseServices(services => services.AddTransient(interfaceType, implementationType));
			return () => app.CreateProxy<T>(implementationType.GetTypeInfo());
		}

		#region Common Classes
		public abstract class Base { }

		internal class Derivative : Base { }
		#endregion
	}
}
