using System;
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

		internal TService GetDirectTransitionImplementation<TService>(TService service)
		{
			/* C#: 
			public sealed class WrapperName : TService, ITransition<TService>
			{
				// AddDirectImplementation...
			}
			*/
			var interfaceType = typeof(TService);
			var typeBuilder = _moduleBuilder.DefineType($"{_assemblyName.Name}+{interfaceType.FullName}", TypeAttributes.Public | TypeAttributes.Sealed);
			typeBuilder.AddInterfaceImplementation(interfaceType);
			typeBuilder.AddInterfaceImplementation(typeof(ITransition<TService>));
			typeBuilder.AddDirectImplementation<TService>();
			var proxy = typeBuilder.CreateType();
#if DEBUG
			_assemblyBuilder.Save($"{_assemblyName.Name}.dll");
#endif
			return (TService)Activator.CreateInstance(proxy, service);
		}

		private AssemblyName CreateAssemblyName()
		{
			var my = GetType();
			return new AssemblyName($"{my.Namespace}.{my.Name}_{Guid.NewGuid().ToString()}");
		}

		private AssemblyBuilder CreateAssemblyBuilder()
		{
			return AppDomain.CurrentDomain.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.RunAndSave);
		}

		private ModuleBuilder CreateModuleBuilder()
		{
			return _assemblyBuilder.DefineDynamicModule(_assemblyName.Name, $"{_assemblyName.Name}.dll");
		}
	}
}
