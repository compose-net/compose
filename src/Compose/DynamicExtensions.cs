using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Compose
{
	internal static class DynamicExtensions
	{
		internal static void AddDirectImplementation<TService>(this TypeBuilder typeBuilder)
		{
			var serviceName = GetRandomString();
			var serviceField = typeBuilder.AddServiceField<TService>(serviceName);
			typeBuilder.AddServiceProperty<TService>(serviceName, serviceField);
			typeBuilder.AddServiceConstructor<TService>(serviceField);
			typeBuilder.AddPropertyImplementations<TService>(serviceField);
			typeBuilder.AddMethodImplementations<TService>(serviceField);
			typeBuilder.AddDirectChangeImplementation<TService>(serviceField);
		}

		internal static string GetRandomString()
		{
			const string Characters = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm";
			var random = new Random();
			return new string(Enumerable.Repeat(Characters, 16).Select(x => x[random.Next(x.Length)]).ToArray());
		}

		internal static void AddDirectChangeImplementation<TService>(this TypeBuilder typeBuilder, FieldBuilder serviceField)
		{
			var methodInfo = typeof(ITransition<TService>).GetMethod("Change");
			var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual);
			var implementationBuilder = methodBuilder.DefineGenericParameters("TImplementation")[0];
			implementationBuilder.SetInterfaceConstraints(typeof(TService));
			methodBuilder.SetParameters(new[] { implementationBuilder });
			methodBuilder.SetReturnType(methodInfo.ReturnType);
            var methodEmitter = methodBuilder.GetILGenerator();
			methodEmitter.Emit(OpCodes.Ldarg_0);
			methodEmitter.Emit(OpCodes.Ldarg_1);
			methodEmitter.Emit(OpCodes.Box, typeof(TService));
			methodEmitter.Emit(OpCodes.Stfld, serviceField);
			methodEmitter.Emit(OpCodes.Ldc_I4_1);
			methodEmitter.Emit(OpCodes.Ret);
			typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
		}

		internal static void AddMethodImplementations<TService>(this TypeBuilder typeBuilder, FieldBuilder serviceField)
		{
			foreach (var methodInfo in typeof(TService).GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(x => !x.IsSpecialName))
				typeBuilder.AddMethodImplementation(methodInfo, serviceField);
		}

		internal static void AddMethodImplementation(this TypeBuilder typeBuilder, MethodInfo methodInfo, FieldBuilder serviceField)
		{
            var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual);
            methodBuilder.SetReturnType(methodInfo.ReturnType);
            methodBuilder.SetParameters(methodInfo.GetParameters().Select(x => x.ParameterType).ToArray());
            if (methodInfo.IsGenericMethod) methodBuilder.AddGenericParameters(methodInfo);
            var methodEmitter = methodBuilder.GetILGenerator();
			methodEmitter.Emit(OpCodes.Ldarg_0);
			methodEmitter.Emit(OpCodes.Ldfld, serviceField);
			for (var argIndex = 1; argIndex <= methodInfo.GetParameters().Length; argIndex++)
				methodEmitter.Emit(OpCodes.Ldarg, argIndex);
			methodEmitter.Emit(OpCodes.Callvirt, methodInfo);
			methodEmitter.Emit(OpCodes.Ret);
			typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
		}

        internal static void AddGenericParameters(this MethodBuilder methodBuilder, MethodInfo methodInfo)
        {
            List<GenericTypeParameterBuilder> parameters = new List<GenericTypeParameterBuilder>();
            var genericInfos = methodInfo.GetGenericArguments().ToArray();
            var genericBuilders = methodBuilder.DefineGenericParameters(genericInfos.Select(x => x.Name).ToArray());
            for (var i = 0; i < genericBuilders.Length; i++)
                parameters.Add(genericBuilders[i].DefineGeneric(genericInfos[i]));
        }

        internal static GenericTypeParameterBuilder DefineGeneric(this GenericTypeParameterBuilder genericBuilder, Type genericType)
        {

            return genericBuilder;
        }

		internal static void AddPropertyImplementations<TService>(this TypeBuilder typeBuilder, FieldBuilder serviceField)
		{
			foreach (var propertyInfo in typeof(TService).GetProperties(BindingFlags.Instance | BindingFlags.Public))
				typeBuilder.AddPropertyImplementation(propertyInfo, serviceField);
		}

		internal static void AddPropertyImplementation(this TypeBuilder typeBuilder, PropertyInfo propertyInfo, FieldBuilder serviceField)
		{
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

		internal static FieldBuilder AddServiceField<TService>(this TypeBuilder typeBuilder, string serviceName)
		{
			return typeBuilder.DefineField($"_{serviceName}", typeof(TService), FieldAttributes.Private);
		}

		internal static ConstructorBuilder AddServiceConstructor<TService>(this TypeBuilder typeBuilder, FieldBuilder fieldBuilder)
		{
			var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(TService) });
			var ctorEmitter = ctorBuilder.GetILGenerator();
			ctorEmitter.Emit(OpCodes.Ldarg_0);
			ctorEmitter.Emit(OpCodes.Ldarg_1);
			ctorEmitter.Emit(OpCodes.Stfld, fieldBuilder);
			ctorEmitter.Emit(OpCodes.Ret);
			return ctorBuilder;
		}

		internal static void AddServiceProperty<TService>(this TypeBuilder typeBuilder, string serviceName, FieldBuilder fieldBuilder)
		{
			var serviceType = typeof(TService);			
			var propertyBuilder = typeBuilder.DefineProperty(serviceName, PropertyAttributes.None, serviceType, null);
			var propertyAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

			var propertyGetMethod = typeBuilder.DefineMethod($"get_{serviceName}", propertyAttributes, serviceType, Type.EmptyTypes);
			var propertyGetEmitter = propertyGetMethod.GetILGenerator();
			propertyGetEmitter.Emit(OpCodes.Ldarg_0);
			propertyGetEmitter.Emit(OpCodes.Ldfld, fieldBuilder);
			propertyGetEmitter.Emit(OpCodes.Ret);

			var propertySetMethod = typeBuilder.DefineMethod($"set_{serviceName}", propertyAttributes, null, new[] { serviceType });
			var propertySetEmitter = propertySetMethod.GetILGenerator();
			propertySetEmitter.Emit(OpCodes.Ldarg_0);
			propertySetEmitter.Emit(OpCodes.Ldarg_1);
			propertySetEmitter.Emit(OpCodes.Stfld, fieldBuilder);
			propertySetEmitter.Emit(OpCodes.Ret);

			propertyBuilder.SetGetMethod(propertyGetMethod);
			propertyBuilder.SetSetMethod(propertySetMethod);
		}
	}
}
