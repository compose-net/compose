using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Compose
{
	internal sealed class DynamicEmitter
	{
		private readonly AssemblyName _assemblyName;
		private readonly AssemblyBuilder _assemblyBuilder;
		private readonly ModuleBuilder _moduleBuilder;

		public DynamicEmitter()
		{
			_assemblyName = CreateAssemblyName();
			_assemblyBuilder = CreateAssemblyBuilder();
			_moduleBuilder = CreateModuleBuilder();
		}

		internal TypeInfo GetManagedDynamicProxy(TypeInfo serviceTypeInfo, TypeInfo injectionTypeInfo)
		{
			var serviceType = serviceTypeInfo.AsType();
            ValidateProxyIsPossible(serviceTypeInfo);
			/* C#: 
			public sealed class WrapperName[<TService>] : TService, ITransition<TService>
			{
				// AddDirectImplementation...
			}
			*/
			var typeBuilder = _moduleBuilder.DefineType($"{_assemblyName.Name}+{serviceType.FullName}", TypeAttributes.Public | TypeAttributes.Sealed);
			typeBuilder.AddInterfaceImplementation(serviceType);
			foreach (var implementedInterface in serviceTypeInfo.ImplementedInterfaces)
				typeBuilder.AddInterfaceImplementation(implementedInterface);
			if (serviceTypeInfo.IsGenericType)
				typeBuilder.AddGenericsFrom(serviceTypeInfo);
			typeBuilder.AddInterfaceImplementation(typeof(ITransition<>).MakeGenericType(serviceType));
			try
			{
				typeBuilder.AddDirectImplementation(serviceTypeInfo, injectionTypeInfo);
				return typeBuilder.CreateTypeInfo();
			}
			catch(Exception ex)
			{
				throw new UnsupportedTypeDefintionException(serviceType, ex);
			}
		}

		private void ValidateProxyIsPossible(TypeInfo serviceType)
		{
			if (!serviceType.IsPublic && !serviceType.IsNestedPublic)
				throw new InaccessibleTypeException(serviceType);
			if (serviceType.IsGenericType)
				ValidateGenericTypesAccessible(serviceType);
		}

		private void ValidateGenericTypesAccessible(TypeInfo serviceType)
		{
			var inaccessibleGeneric = serviceType.GenericTypeArguments
				.Select(x => x.GetTypeInfo())
				.FirstOrDefault(x => !x.IsPublic && !x.IsNestedPublic);
			if (inaccessibleGeneric != null)
				throw new InaccessibleTypeException(serviceType, inaccessibleGeneric);
		}

		private AssemblyName CreateAssemblyName()
		{
			var my = GetType();
			return new AssemblyName($"{my.Namespace}.{my.Name}_{Guid.NewGuid().ToString()}");
		}

		private AssemblyBuilder CreateAssemblyBuilder()
		{
			return AssemblyBuilder.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.Run);
		}

		private ModuleBuilder CreateModuleBuilder()
		{
			return _assemblyBuilder.DefineDynamicModule(_assemblyName.Name);
		}
	}
}
