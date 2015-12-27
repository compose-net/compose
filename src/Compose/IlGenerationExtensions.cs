using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Compose
{
	internal static class IlGenerationExtensions
	{
		private static readonly Random Random = new Random();

		internal static void AddDirectImplementation(this TypeBuilder typeBuilder, TypeInfo serviceTypeInfo, TypeInfo managerTypeInfo)
		{
			var managerFieldName = GetRandomString();
			var managerType = managerTypeInfo.AsType();
			var managerField = typeBuilder.AddManagerField(managerFieldName, managerType);
			var managerCurrent = managerTypeInfo.GetDeclaredProperty("CurrentService").GetMethod;
			var managerRegister = managerTypeInfo.GetDeclaredMethod("Register");
			var implementedInterfaces = new[] { serviceTypeInfo }.Union(serviceTypeInfo.ImplementedInterfaces.Select(x => x.GetTypeInfo()).ToArray()).ToArray();
			typeBuilder.AddServiceConstructor(managerField, managerType, managerRegister);
			typeBuilder.AddPropertyImplementations(managerField, managerCurrent, implementedInterfaces);
			typeBuilder.AddMethodImplementations(managerField, managerCurrent, implementedInterfaces);
		}

		private static string GetRandomString()
		{
			const string characters = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm";
			return new string(Enumerable.Repeat(characters, 16).Select(x => x[Random.Next(x.Length)]).ToArray());
		}

		internal static void AddGenericsFrom(this TypeBuilder typeBuilder, TypeInfo serviceType)
		{
			var serviceGenerics = serviceType.GenericTypeArguments.Select(x => x.GetTypeInfo()).ToArray();
			var genericBuilders = typeBuilder.DefineGenericParameters(serviceGenerics.Select(x => x.Name).ToArray());
			for (var i = 0; i < genericBuilders.Length; i++)
				if (serviceGenerics[i].IsGenericParameter)
					genericBuilders[i].DefineGeneric(serviceGenerics[i], serviceGenerics, serviceType, false);
		}

		private static void AddMethodImplementations(this TypeBuilder typeBuilder, FieldBuilder managerField, MethodInfo managerCurrent, TypeInfo[] implementedInterfaces)
		{
			foreach (var implementedInterface in implementedInterfaces)
				foreach (var methodInfo in implementedInterface.DeclaredMethods.Where(x => x.IsPublic && !x.IsStatic && !x.IsSpecialName))
					ExceptionHelpers.ReThrow(typeBuilder.AddMethodImplementation, methodInfo, managerField, managerCurrent, implementedInterface, inner => new UnsupportedMethodDefinitionException(methodInfo, inner));
		}

		private static void AddMethodImplementation(this TypeBuilder typeBuilder, MethodInfo methodInfo, FieldBuilder managerField, MethodInfo managerCurrent, TypeInfo serviceType)
		{
			/* C#: 
			public virtual ReturnType MethodName[<Generics>]([Args]) [where Generic : Constraints]
			{
				[return] this._TManagerField.CurrentService.MethodName([Args]);
			}
			*/
			var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual);
			methodBuilder.SetReturnType(methodInfo.ReturnType);
			methodBuilder.SetParameters(methodInfo.GetParameters().Select(x => x.ParameterType).ToArray());
			if (methodInfo.IsGenericMethod) methodBuilder.AddGenericParameters(methodInfo, serviceType);
			var methodEmitter = methodBuilder.GetILGenerator();
			methodEmitter.Emit(OpCodes.Ldarg_0);
			methodEmitter.Emit(OpCodes.Ldfld, managerField);
			methodEmitter.Emit(OpCodes.Callvirt, managerCurrent);
			for (var argIndex = 1; argIndex <= methodInfo.GetParameters().Length; argIndex++)
				methodEmitter.Emit(OpCodes.Ldarg, argIndex);
			methodEmitter.Emit(OpCodes.Callvirt, methodInfo);
			methodEmitter.Emit(OpCodes.Ret);
			typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
		}

		private static void AddGenericParameters(this MethodBuilder methodBuilder, MethodInfo methodInfo, TypeInfo serviceType)
		{
			var genericInfos = methodInfo.GetGenericArguments().Select(x => x.GetTypeInfo()).ToArray();
			var genericBuilders = methodBuilder.DefineGenericParameters(genericInfos.Select(x => x.Name).ToArray());
			for (var i = 0; i < genericBuilders.Length; i++)
				genericBuilders[i].DefineGeneric(genericInfos[i], genericInfos, serviceType, true);
		}

		private static GenericTypeParameterBuilder DefineGeneric(this GenericTypeParameterBuilder genericBuilder, TypeInfo genericType, TypeInfo[] methodGenerics, TypeInfo serviceType, bool includeVariance)
		{
			var constraints = genericType.GetGenericParameterConstraints();
			genericBuilder.SetInterfaceConstraints(
				constraints.Where(x => x.GetTypeInfo().IsInterface).Union(
				constraints.Where(x => x.IsGenericParameter).Select(x => x.GetTypeInfo().GetUnderlyingGenericType(methodGenerics, serviceType).AsType()
			)).ToArray());
			genericBuilder.SetBaseTypeConstraint(genericType.BaseType);
			if (includeVariance) genericBuilder.SetGenericParameterAttributes(genericType.GenericParameterAttributes);
			else genericBuilder.SetGenericParameterAttributes(genericType.GenericParameterAttributes & ~GenericParameterAttributes.Contravariant & ~GenericParameterAttributes.Covariant);
			return genericBuilder;
		}

		private static TypeInfo GetUnderlyingGenericType(this TypeInfo constraint, TypeInfo[] methodGenerics, TypeInfo serviceType)
		{
			if (methodGenerics.Contains(constraint)) return constraint;

			var typedDefinitions = serviceType.GenericTypeArguments.Select(x => x.GetTypeInfo()).ToArray();
			var genericDefinitions = serviceType.IsGenericType ? serviceType.GetGenericTypeDefinition().GetTypeInfo().GetGenericArguments() : new TypeInfo[0];

			for (var i = 0; i < genericDefinitions.Length; i++)
				if (genericDefinitions[i] == constraint)
					return typedDefinitions[i];

			throw new NotSupportedException("Generic constraint is not supported.");
		}

		private static void AddPropertyImplementations(this TypeBuilder typeBuilder, FieldBuilder managerField, MethodInfo managerCurrent, TypeInfo[] implementedInterfaces)
		{
			foreach (var implementedInterface in implementedInterfaces)
				foreach (var propertyInfo in implementedInterface.DeclaredProperties)
					ExceptionHelpers.ReThrow(typeBuilder.AddPropertyImplementation, propertyInfo, managerField, managerCurrent, inner => new UnsupportedPropertyDefinitionException(propertyInfo, inner));
		}

		private static void AddPropertyImplementation(this TypeBuilder typeBuilder, PropertyInfo propertyInfo, FieldBuilder managerField, MethodInfo managerCurrent)
		{
			/* C#: 
			public virtual ReturnType PropertyName
			{
				get { return this._TServiceField.PropertyName; }
				set { this._TServiceField.PropertyName = value; }
			}
			*/
			var propertyBuilder = typeBuilder.DefineProperty(propertyInfo.Name, propertyInfo.Attributes, propertyInfo.PropertyType, null);
			if (propertyInfo.CanRead) typeBuilder.AddPropertyGetImplementation(propertyBuilder, propertyInfo, managerField, managerCurrent);
			if (propertyInfo.CanWrite) typeBuilder.AddPropertySetImplementation(propertyBuilder, propertyInfo, managerField, managerCurrent);
		}

		private static void AddPropertyGetImplementation(this TypeBuilder typeBuilder, PropertyBuilder propertyBuilder, PropertyInfo propertyInfo, FieldBuilder managerField, MethodInfo managerCurrent)
		{
			var propertyInfoGetMethod = propertyInfo.GetMethod;
			if (propertyInfoGetMethod == null) return;
			var propertyGetMethod = typeBuilder.DefineMethod(propertyInfoGetMethod.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual, propertyInfo.PropertyType, Type.EmptyTypes);
			var propertyGetEmitter = propertyGetMethod.GetILGenerator();
			propertyGetEmitter.Emit(OpCodes.Ldarg_0);
			propertyGetEmitter.Emit(OpCodes.Ldfld, managerField);
			propertyGetEmitter.Emit(OpCodes.Callvirt, managerCurrent);
			propertyGetEmitter.Emit(OpCodes.Callvirt, propertyInfoGetMethod);
			propertyGetEmitter.Emit(OpCodes.Ret);
			propertyBuilder.SetGetMethod(propertyGetMethod);
			typeBuilder.DefineMethodOverride(propertyGetMethod, propertyInfoGetMethod);
		}

		private static void AddPropertySetImplementation(this TypeBuilder typeBuilder, PropertyBuilder propertyBuilder, PropertyInfo propertyInfo, FieldBuilder managerField, MethodInfo managerCurrent)
		{
			var propertyInfoSetMethod = propertyInfo.SetMethod;
			if (propertyInfoSetMethod == null) return;
			var propertySetMethod = typeBuilder.DefineMethod(propertyInfoSetMethod.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Virtual, null, new[] { propertyInfo.PropertyType });
			var propertySetEmitter = propertySetMethod.GetILGenerator();
			propertySetEmitter.Emit(OpCodes.Ldarg_0);
			propertySetEmitter.Emit(OpCodes.Ldfld, managerField);
			propertySetEmitter.Emit(OpCodes.Callvirt, managerCurrent);
			propertySetEmitter.Emit(OpCodes.Ldarg_1);
			propertySetEmitter.Emit(OpCodes.Callvirt, propertyInfoSetMethod);
			propertySetEmitter.Emit(OpCodes.Ret);
			propertyBuilder.SetSetMethod(propertySetMethod);
			typeBuilder.DefineMethodOverride(propertySetMethod, propertyInfoSetMethod);
		}

		private static FieldBuilder AddManagerField(this TypeBuilder typeBuilder, string fieldName, Type managerType)
		{
			return typeBuilder.DefineField($"_{fieldName}", managerType, FieldAttributes.Private);
		}

		private static ConstructorBuilder AddServiceConstructor(this TypeBuilder typeBuilder, FieldBuilder managerField, Type managerType, MethodInfo managerRegister)
		{
			/* C#: 
			public Constructor(TInjection arg1)
			{
				this._TInjectionField = arg1;
			}
			*/
			var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { managerType });
			var ctorEmitter = ctorBuilder.GetILGenerator();
			ctorEmitter.Emit(OpCodes.Ldarg_0);
			ctorEmitter.Emit(OpCodes.Ldarg_1);
			ctorEmitter.Emit(OpCodes.Stfld, managerField);
			ctorEmitter.Emit(OpCodes.Ldarg_1);
			ctorEmitter.Emit(OpCodes.Ldarg_0);
			ctorEmitter.Emit(OpCodes.Callvirt, managerRegister);
			ctorEmitter.Emit(OpCodes.Ret);
			return ctorBuilder;
		}
	}
}
