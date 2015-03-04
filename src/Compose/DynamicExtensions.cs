using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Compose
{
	internal static class DynamicExtensions
	{
		private static Random random = new Random();

		internal static void AddDirectImplementation(this TypeBuilder typeBuilder, Type serviceType)
		{
			var serviceName = GetRandomString();
			var serviceField = typeBuilder.AddServiceField(serviceName, serviceType);
			typeBuilder.AddServiceConstructor(serviceField, serviceType);
			typeBuilder.AddPropertyImplementations(serviceField, serviceType);
			typeBuilder.AddMethodImplementations(serviceField, serviceType);
			typeBuilder.AddDirectChangeImplementation(serviceField, serviceType);
		}

		internal static string GetRandomString()
		{
			const string Characters = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm";
			return new string(Enumerable.Repeat(Characters, 16).Select(x => x[random.Next(x.Length)]).ToArray());
		}

		internal static void AddDirectChangeImplementation(this TypeBuilder typeBuilder, FieldBuilder serviceField, Type serviceType)
		{
			/* C#: 
			public virtual bool Change(TService arg1)
			{
				this._TServiceField = arg1;
				return true;
			}
			*/
			var methodInfo = typeof(ITransition<>).MakeGenericType(serviceType).GetMethod("Change");
			var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, methodInfo.ReturnType, new[] { serviceType });
            var methodEmitter = methodBuilder.GetILGenerator();
			methodEmitter.Emit(OpCodes.Ldarg_0);
			methodEmitter.Emit(OpCodes.Ldarg_1);
			methodEmitter.Emit(OpCodes.Stfld, serviceField);
			methodEmitter.Emit(OpCodes.Ldc_I4_1);
			methodEmitter.Emit(OpCodes.Ret);
			typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
		}

		internal static void AddMethodImplementations(this TypeBuilder typeBuilder, FieldBuilder serviceField, Type serviceType)
		{
			foreach (var methodInfo in serviceType.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(x => !x.IsSpecialName))
				ExceptionHelpers.ReThrow(typeBuilder.AddMethodImplementation, methodInfo, serviceField, serviceType, inner => new UnsupportedMethodDefinitionException(methodInfo, inner));
		}

		internal static void AddMethodImplementation(this TypeBuilder typeBuilder, MethodInfo methodInfo, FieldBuilder serviceField, Type serviceType)
		{
			/* C#: 
			public virtual ReturnType MethodName[<Generics>]([Args]) [where Generic : Constraints]
			{
				[return] this._TServiceField.MethodName([Args]);
			}
			*/
			var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual);
            methodBuilder.SetReturnType(methodInfo.ReturnType);
            methodBuilder.SetParameters(methodInfo.GetParameters().Select(x => x.ParameterType).ToArray());
            if (methodInfo.IsGenericMethod) methodBuilder.AddGenericParameters(methodInfo, serviceType);
            var methodEmitter = methodBuilder.GetILGenerator();
			methodEmitter.Emit(OpCodes.Ldarg_0);
			methodEmitter.Emit(OpCodes.Ldfld, serviceField);
			for (var argIndex = 1; argIndex <= methodInfo.GetParameters().Length; argIndex++)
				methodEmitter.Emit(OpCodes.Ldarg, argIndex);
			methodEmitter.Emit(OpCodes.Callvirt, methodInfo);
			methodEmitter.Emit(OpCodes.Ret);
			typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
		}

        internal static void AddGenericParameters(this MethodBuilder methodBuilder, MethodInfo methodInfo, Type serviceType)
        {
            var genericInfos = methodInfo.GetGenericArguments().ToArray();
            var genericBuilders = methodBuilder.DefineGenericParameters(genericInfos.Select(x => x.Name).ToArray());
            for (var i = 0; i < genericBuilders.Length; i++)
                genericBuilders[i].DefineGeneric(genericInfos[i], serviceType);
        }

        internal static GenericTypeParameterBuilder DefineGeneric(this GenericTypeParameterBuilder genericBuilder, Type genericType, Type serviceType)
        {
            var constraints = genericType.GetGenericParameterConstraints();
            genericBuilder.SetInterfaceConstraints(
				constraints.Where(x => x.IsInterface).Union(
				constraints.Where(x => x.IsGenericParameter).Select(x => x.GetUnderlyingGenericType(serviceType)
			)).ToArray());
			genericBuilder.SetBaseTypeConstraint(genericType.BaseType);
			genericBuilder.SetGenericParameterAttributes(genericType.GenericParameterAttributes);
            return genericBuilder;
        }

		internal static Type GetUnderlyingGenericType(this Type constraint, Type serviceType)
		{
			var typedDefintions = serviceType.GetGenericArguments();
			var genericDefinitions = serviceType.GetGenericTypeDefinition().GetGenericArguments();

			for (var i = 0; i < genericDefinitions.Length; i++)
				if (genericDefinitions[i] == constraint)
					return typedDefintions[i];
			throw new NotSupportedException();
		}

		internal static void AddPropertyImplementations(this TypeBuilder typeBuilder, FieldBuilder serviceField, Type serviceType)
		{
			foreach (var propertyInfo in serviceType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
				ExceptionHelpers.ReThrow(typeBuilder.AddPropertyImplementation, propertyInfo, serviceField, inner => new UnsupportedPropertyDefinitionException(propertyInfo, inner));
		}

		internal static void AddPropertyImplementation(this TypeBuilder typeBuilder, PropertyInfo propertyInfo, FieldBuilder serviceField)
		{
			/* C#: 
			public virtual ReturnType PropertyName
			{
				get { return this._TServiceField.PropertyName; }
				set { this._TServiceField.PropertyName = value; }
			}
			*/
			var propertyBuilder = typeBuilder.DefineProperty(propertyInfo.Name, propertyInfo.Attributes, propertyInfo.PropertyType, null);
			if (propertyInfo.CanRead) typeBuilder.AddPropertyGetImplementation(propertyBuilder, propertyInfo, serviceField);
			if (propertyInfo.CanWrite) typeBuilder.AddPropertySetImplementation(propertyBuilder, propertyInfo, serviceField);
		}

		internal static void AddPropertyGetImplementation(this TypeBuilder typeBuilder, PropertyBuilder propertyBuilder, PropertyInfo propertyInfo, FieldBuilder serviceField)
		{
			var propertyInfoGetMethod = propertyInfo.GetGetMethod();
			var propertyGetMethod = typeBuilder.DefineMethod(propertyInfoGetMethod.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual, propertyInfo.PropertyType, Type.EmptyTypes);
			var propertyGetEmitter = propertyGetMethod.GetILGenerator();
			propertyGetEmitter.Emit(OpCodes.Ldarg_0);
			propertyGetEmitter.Emit(OpCodes.Ldfld, serviceField);
			propertyGetEmitter.Emit(OpCodes.Callvirt, propertyInfoGetMethod);
			propertyGetEmitter.Emit(OpCodes.Ret);
			propertyBuilder.SetGetMethod(propertyGetMethod);
			typeBuilder.DefineMethodOverride(propertyGetMethod, propertyInfoGetMethod);
		}

		internal static void AddPropertySetImplementation(this TypeBuilder typeBuilder, PropertyBuilder propertyBuilder, PropertyInfo propertyInfo, FieldBuilder serviceField)
		{
			var propertyInfoSetMethod = propertyInfo.GetSetMethod();
			var propertySetMethod = typeBuilder.DefineMethod(propertyInfoSetMethod.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual, null, new[] { propertyInfo.PropertyType });
			var propertySetEmitter = propertySetMethod.GetILGenerator();
			propertySetEmitter.Emit(OpCodes.Ldarg_0);
			propertySetEmitter.Emit(OpCodes.Ldfld, serviceField);
			propertySetEmitter.Emit(OpCodes.Ldarg_1);
			propertySetEmitter.Emit(OpCodes.Callvirt, propertyInfoSetMethod);
			propertySetEmitter.Emit(OpCodes.Ret);
			propertyBuilder.SetSetMethod(propertySetMethod);
			typeBuilder.DefineMethodOverride(propertySetMethod, propertyInfoSetMethod);
		}

		internal static FieldBuilder AddServiceField(this TypeBuilder typeBuilder, string serviceName, Type serviceType)
		{
			return typeBuilder.DefineField($"_{serviceName}", serviceType, FieldAttributes.Private);
		}

		internal static ConstructorBuilder AddServiceConstructor(this TypeBuilder typeBuilder, FieldBuilder fieldBuilder, Type serviceType)
		{
			/* C#: 
			public Constructor(TService arg1)
			{
				this._TServiceField = arg1;
			}
			*/
			var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { serviceType });
			var ctorEmitter = ctorBuilder.GetILGenerator();
			ctorEmitter.Emit(OpCodes.Ldarg_0);
			ctorEmitter.Emit(OpCodes.Ldarg_1);
			ctorEmitter.Emit(OpCodes.Stfld, fieldBuilder);
			ctorEmitter.Emit(OpCodes.Ret);
			return ctorBuilder;
		}
	}
}
