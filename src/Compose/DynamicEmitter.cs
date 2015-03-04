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

		internal Type GetDirectTransitionImplementation(Type serviceType)
		{
			/* C#: 
			public sealed class WrapperName : TService, ITransition<TService>
			{
				// AddDirectImplementation...
			}
			*/
			var typeBuilder = _moduleBuilder.DefineType($"{_assemblyName.Name}+{serviceType.FullName}", TypeAttributes.Public | TypeAttributes.Sealed);
			typeBuilder.AddInterfaceImplementation(serviceType);
			typeBuilder.AddInterfaceImplementation(typeof(ITransition<>).MakeGenericType(serviceType));
			try
			{
				typeBuilder.AddDirectImplementation(serviceType);
#if DEBUG
				var type = typeBuilder.CreateType();
				_assemblyBuilder.Save($"{_assemblyName.Name}.dll");
				return type;
			}
#else
				return typeBuilder.CreateType();
			}
#endif
			catch(Exception ex)
			{
				throw new UnsupportedClassDefintionException(serviceType, ex);
			}
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
