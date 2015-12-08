using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Compose
{
	internal sealed class IlGeneratingDynamicEmitter : DynamicEmitter
	{
		public const string AssemblyName = "Compose.DynamicProxies";

		private readonly AssemblyName _assemblyName;
		private readonly AssemblyBuilder _assemblyBuilder;
		private readonly ModuleBuilder _moduleBuilder;

		public IlGeneratingDynamicEmitter()
		{
			_assemblyName = CreateAssemblyName();
			_assemblyBuilder = CreateAssemblyBuilder();
			_moduleBuilder = CreateModuleBuilder();
		}

		public Type GetManagedDynamicProxy(TypeInfo serviceTypeInfo)
		{
			var serviceType = serviceTypeInfo.AsType();
			ValidateProxyIsPossible(serviceTypeInfo, true);
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
			try
			{
				var managerTypeInfo = typeof(DynamicRegister<>).MakeGenericType(serviceType).GetTypeInfo();
				typeBuilder.AddDirectImplementation(serviceTypeInfo, managerTypeInfo);
				return typeBuilder.CreateTypeInfo().AsType();
			}
			catch (Exception ex)
			{
				ValidateProxyIsPossible(serviceTypeInfo, false);
				throw new UnsupportedTypeDefinitionException(serviceType, ex);
			}
		}

		private void ValidateProxyIsPossible(TypeInfo serviceType, bool acceptInternalsVisibleTo)
		{
			if (!serviceType.IsAccessible(acceptInternalsVisibleTo))
				throw new InaccessibleTypeException(serviceType);
			if (serviceType.IsGenericType)
				ValidateGenericTypesAccessible(serviceType, acceptInternalsVisibleTo);
		}

		private void ValidateGenericTypesAccessible(TypeInfo serviceType, bool acceptInternalsVisibleTo)
		{
			var inaccessibleGeneric = serviceType.GenericTypeArguments
				.Select(x => x.GetTypeInfo())
				.FirstOrDefault(x => !x.IsAccessible(acceptInternalsVisibleTo));
			if (inaccessibleGeneric != null)
				throw new InaccessibleTypeException(serviceType, inaccessibleGeneric);
		}

		private AssemblyName CreateAssemblyName()
		{
			return new AssemblyName(AssemblyName);
		}

		private AssemblyBuilder CreateAssemblyBuilder()
		{
			var attributes = new List<CustomAttributeBuilder>(7)
			{
				new CustomAttributeBuilder(typeof (AssemblyTitleAttribute).GetTypeInfo().DeclaredConstructors.Single(),
					new object[] {"Compose.DynamicProxies"}),
				new CustomAttributeBuilder(typeof (AssemblyCompanyAttribute).GetTypeInfo().DeclaredConstructors.Single(),
					new object[] {"Devbot.Net"}),
				new CustomAttributeBuilder(typeof (AssemblyProductAttribute).GetTypeInfo().DeclaredConstructors.Single(),
					new object[] {"Compose.DynamicProxies"}),
				new CustomAttributeBuilder(typeof (AssemblyCopyrightAttribute).GetTypeInfo().DeclaredConstructors.Single(),
					new object[] {$"Devbot.Net {DateTime.Now.Year}"}),
				new CustomAttributeBuilder(typeof (ComVisibleAttribute).GetTypeInfo().DeclaredConstructors.Single(),
					new object[] {false}),
				new CustomAttributeBuilder(typeof (TargetFrameworkAttribute).GetTypeInfo().DeclaredConstructors.Single(),
					new object[] {".NETFramework,Version=v4.5"})
			};
			return AssemblyBuilder.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.Run, attributes);
		}

		private ModuleBuilder CreateModuleBuilder()
		{
			return _assemblyBuilder.DefineDynamicModule(_assemblyName.Name);
		}
	}
}
