using System;
using System.Reflection;

namespace Compose
{
	internal sealed class DynamicActivator
	{
		private readonly Func<TypeInfo> _dynamicTypeFactory;
		private readonly Func<object> _dynamicManagerFactory;

		public DynamicActivator(Func<TypeInfo> dynamicTypeFactory, Func<object> dynamicManagerFactor)
		{
			_dynamicTypeFactory = dynamicTypeFactory;
			_dynamicManagerFactory = dynamicManagerFactor;
		}

		public object Create()
			=> Activate.Type(_dynamicTypeFactory(), _dynamicManagerFactory());
	}
}
