using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;

namespace Compose
{
	internal sealed class DynamicEmitter
	{
		public const string AssemblyName = "Compose.DynamicProxies";

		private readonly AssemblyName _assemblyName;
		private readonly AssemblyBuilder _assemblyBuilder;
		private readonly ModuleBuilder _moduleBuilder;

		public DynamicEmitter()
		{
			_assemblyName = CreateAssemblyName();
			_assemblyBuilder = CreateAssemblyBuilder();
			_moduleBuilder = CreateModuleBuilder();
		}

		internal Type GetManagedDynamicProxy(TypeInfo serviceTypeInfo)
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
				var managerTypeInfo = typeof(IDynamicRegister<>).MakeGenericType(serviceType).GetTypeInfo();
                typeBuilder.AddDirectImplementation(serviceTypeInfo, managerTypeInfo);

#if DEBUG && !NETCORE45
				var dynamicType = typeBuilder.CreateType();
                //_assemblyBuilder.Save($"{_assemblyName.Name}.dll");
				return dynamicType;
#else
                return typeBuilder.CreateTypeInfo().AsType();
#endif
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
			var attributes = new List<CustomAttributeBuilder>(7);
			attributes.Add(new CustomAttributeBuilder(typeof(AssemblyTitleAttribute).GetConstructors().Single(), new object[] { "Compose.DynamicProxies" }));
			attributes.Add(new CustomAttributeBuilder(typeof(AssemblyCompanyAttribute).GetConstructors().Single(), new object[] { "Devbot.Net" }));
			attributes.Add(new CustomAttributeBuilder(typeof(AssemblyProductAttribute).GetConstructors().Single(), new object[] { "Compose.DynamicProxies" }));
			attributes.Add(new CustomAttributeBuilder(typeof(AssemblyCopyrightAttribute).GetConstructors().Single(), new object[] { $"Devbot.Net {DateTime.Now.Year}" }));
			attributes.Add(new CustomAttributeBuilder(typeof(ComVisibleAttribute).GetConstructors().Single(), new object[] { false }));
			attributes.Add(new CustomAttributeBuilder(typeof(TargetFrameworkAttribute).GetConstructors().Single(), new object[] { ".NETFramework,Version=v4.5" }));
#if DEBUG && !NETCORE45
			attributes.Add(new CustomAttributeBuilder(typeof(DebuggableAttribute).GetConstructor(new[] { typeof(DebuggableAttribute.DebuggingModes) }), new object[] { DebuggableAttribute.DebuggingModes.Default | DebuggableAttribute.DebuggingModes.DisableOptimizations | DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints | DebuggableAttribute.DebuggingModes.EnableEditAndContinue }));
            return AppDomain.CurrentDomain.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.RunAndSave, attributes, SecurityContextSource.CurrentAppDomain);
#else
			return AssemblyBuilder.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.Run, attributes);
#endif
		}

		private ModuleBuilder CreateModuleBuilder()
		{
			return _assemblyBuilder.DefineDynamicModule(_assemblyName.Name);
		}
	}
}
