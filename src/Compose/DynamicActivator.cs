using System;

namespace Compose
{
	internal sealed class DynamicActivator
	{
		private readonly Func<Type> _dynamicTypeFactory;
		private readonly Func<object> _dynamicManagerFactory;

		public DynamicActivator(Func<Type> dynamicTypeFactory, Func<object> dynamicManagerFactor)
		{
			_dynamicTypeFactory = dynamicTypeFactory;
			_dynamicManagerFactory = dynamicManagerFactor;
		}

		public object Create()
			=> Activator.CreateInstance(_dynamicTypeFactory(), _dynamicManagerFactory());
	}
}
