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
			var snapshotName = GetRandomString();
			var serviceField = typeBuilder.AddServiceField(serviceType, serviceName);
			var snapshotField = typeBuilder.AddSnapshotField(serviceType, snapshotName);
#if DEBUG
            var idName = GetRandomString();
            var idField = typeBuilder.AddIdField(idName);
            typeBuilder.AddServiceConstructor(serviceType, serviceField, idField);
#else
            typeBuilder.AddServiceConstructor(serviceType, serviceField);
#endif
            typeBuilder.AddPropertyImplementations(serviceType, serviceField);
			typeBuilder.AddMethodImplementations(serviceType, serviceField);
			typeBuilder.AddChangeImplementation(serviceType, serviceField);
			typeBuilder.AddSnapshotImplementation(snapshotField, serviceField);
			typeBuilder.AddRestoreImplementation(snapshotField, serviceField);
        }

		private static string GetRandomString()
		{
			const string Characters = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm";
			return new string(Enumerable.Repeat(Characters, 16).Select(x => x[random.Next(x.Length)]).ToArray());
		}

        private static FieldBuilder AddServiceField(this TypeBuilder typeBuilder, Type serviceType, string serviceName)
        {
            return typeBuilder.DefineField($"_{serviceName}", serviceType, FieldAttributes.Private);
        }

        private static FieldBuilder AddSnapshotField(this TypeBuilder typeBuilder, Type serviceType, string snapshotName)
        {
            return typeBuilder.DefineField($"_{snapshotName}", serviceType, FieldAttributes.Private);
        }

#if DEBUG
        private static FieldBuilder AddIdField(this TypeBuilder typeBuilder, string idName)
        {
            return typeBuilder.DefineField($"_{idName}", typeof(Guid), FieldAttributes.Private);
        }

        private static ConstructorBuilder AddServiceConstructor(this TypeBuilder typeBuilder, Type serviceType, FieldBuilder fieldBuilder, FieldBuilder idField)
#else
        private static ConstructorBuilder AddServiceConstructor(this TypeBuilder typeBuilder, FieldBuilder fieldBuilder, Type serviceType)
#endif
        {
            /* C#: 
			public Constructor(TService arg1)
			{
            #if DEBUG
                this.GuidId = Guid.NewGuid();
            #endif
				this._TServiceField = arg1;
			}
			*/
            var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { serviceType });
            var ctorEmitter = ctorBuilder.GetILGenerator();
            var newGuidMethod = typeof(Guid).GetMethod("NewGuid");
#if DEBUG
            ctorEmitter.Emit(OpCodes.Ldarg_0);
            ctorEmitter.Emit(OpCodes.Call, newGuidMethod);
            ctorEmitter.Emit(OpCodes.Stfld, idField);
#endif
            ctorEmitter.Emit(OpCodes.Ldarg_0);
            ctorEmitter.Emit(OpCodes.Ldarg_1);
            ctorEmitter.Emit(OpCodes.Stfld, fieldBuilder);
            ctorEmitter.Emit(OpCodes.Ret);
            return ctorBuilder;
        }

        private static void AddPropertyImplementations(this TypeBuilder typeBuilder, Type serviceType, FieldBuilder serviceField)
        {
            foreach (var propertyInfo in serviceType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                ExceptionHelpers.ReThrow(typeBuilder.AddPropertyImplementation, propertyInfo, serviceField, inner => new UnsupportedPropertyDefinitionException(propertyInfo, inner));
        }

        private static void AddMethodImplementations(this TypeBuilder typeBuilder, Type serviceType, FieldBuilder serviceField)
        {
            foreach (var methodInfo in serviceType.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(x => !x.IsSpecialName))
                ExceptionHelpers.ReThrow(typeBuilder.AddMethodImplementation, methodInfo, serviceField, serviceType, inner => new UnsupportedMethodDefinitionException(methodInfo, inner));
        }

        private static void AddChangeImplementation(this TypeBuilder typeBuilder, Type serviceType, FieldBuilder serviceField)
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

        private static void AddSnapshotImplementation(this TypeBuilder typeBuilder, FieldBuilder snapshotField, FieldBuilder serviceField)
		{
			/* C#:
			public virtual void Snapshot()
			{
				this._TSnapshotField = this._TServiceField;
			}
			*/
			var methodInfo = typeof(ITransition).GetMethod("Snapshot");
			var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, methodInfo.ReturnType, Type.EmptyTypes);
			var methodEmitter = methodBuilder.GetILGenerator();
			methodEmitter.Emit(OpCodes.Ldarg_0);
			methodEmitter.Emit(OpCodes.Ldarg_0);
			methodEmitter.Emit(OpCodes.Ldfld, serviceField);
			methodEmitter.Emit(OpCodes.Stfld, snapshotField);
			methodEmitter.Emit(OpCodes.Ret);
		}

        private static void AddRestoreImplementation(this TypeBuilder typeBuilder, FieldBuilder snapshotField, FieldBuilder serviceField)
		{
			var methodInfo = typeof(ITransition).GetMethod("Restore");
			var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, methodInfo.ReturnType, Type.EmptyTypes);
			var methodEmitter = methodBuilder.GetILGenerator();
			methodEmitter.Emit(OpCodes.Ldarg_0);
			methodEmitter.Emit(OpCodes.Ldarg_0);
			methodEmitter.Emit(OpCodes.Ldfld, snapshotField);
			methodEmitter.Emit(OpCodes.Stfld, serviceField);
			methodEmitter.Emit(OpCodes.Ret);
		}

        private static void AddMethodImplementation(this TypeBuilder typeBuilder, MethodInfo methodInfo, FieldBuilder serviceField, Type serviceType)
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

        private static void AddGenericParameters(this MethodBuilder methodBuilder, MethodInfo methodInfo, Type serviceType)
        {
            var genericInfos = methodInfo.GetGenericArguments().ToArray();
            var genericBuilders = methodBuilder.DefineGenericParameters(genericInfos.Select(x => x.Name).ToArray());
            for (var i = 0; i < genericBuilders.Length; i++)
                genericBuilders[i].DefineGeneric(genericInfos[i], genericInfos, serviceType);
        }

        private static GenericTypeParameterBuilder DefineGeneric(this GenericTypeParameterBuilder genericBuilder, Type genericType, Type[] methodGenerics, Type serviceType)
        {
            var constraints = genericType.GetGenericParameterConstraints();
            genericBuilder.SetInterfaceConstraints(
				constraints.Where(x => x.IsInterface).Union(
				constraints.Where(x => x.IsGenericParameter).Select(x => x.GetUnderlyingGenericType(methodGenerics, serviceType)
			)).ToArray());
			genericBuilder.SetBaseTypeConstraint(genericType.BaseType);
			genericBuilder.SetGenericParameterAttributes(genericType.GenericParameterAttributes);
            return genericBuilder;
        }

        private static Type GetUnderlyingGenericType(this Type constraint, Type[] methodGenerics, Type serviceType)
		{
			if (methodGenerics.Contains(constraint)) return constraint;

			var typedDefintions = serviceType.GetGenericArguments();
			var genericDefinitions = serviceType.IsGenericType ? serviceType.GetGenericTypeDefinition().GetGenericArguments() : new Type[0];

			for (var i = 0; i < genericDefinitions.Length; i++)
				if (genericDefinitions[i] == constraint)
					return typedDefintions[i];

			throw new NotSupportedException("Generic constraint is not supported.");
		}

        private static void AddPropertyImplementation(this TypeBuilder typeBuilder, PropertyInfo propertyInfo, FieldBuilder serviceField)
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

        private static void AddPropertyGetImplementation(this TypeBuilder typeBuilder, PropertyBuilder propertyBuilder, PropertyInfo propertyInfo, FieldBuilder serviceField)
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

        private static void AddPropertySetImplementation(this TypeBuilder typeBuilder, PropertyBuilder propertyBuilder, PropertyInfo propertyInfo, FieldBuilder serviceField)
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
	}
}
