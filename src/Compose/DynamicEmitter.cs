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
			try
			{
				var managerTypeInfo = typeof(IDynamicRegister<>).MakeGenericType(serviceType).GetTypeInfo();
                typeBuilder.AddDirectImplementation(serviceTypeInfo, managerTypeInfo);

#if DEBUG && !DNXCORE
				var dynamicType = typeBuilder.CreateType();
                //_assemblyBuilder.Save($"{_assemblyName.Name}.dll");
				return dynamicType;
#else
				return typeBuilder.CreateTypeInfo().AsType();
#endif
			}
			catch (Exception ex)
			{
				throw new UnsupportedTypeDefinitionException(serviceType, ex);
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
			//var my = GetType();
			//return new AssemblyName($"{my.Namespace}.{my.Name}_{Guid.NewGuid().ToString()}");
			return new AssemblyName("Compose.DynamicProxies");
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
#if DEBUG && !DNXCORE
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
