using System;

namespace Compose
{
	internal interface ISingletonRepositoryServiceProvider : IExtendableServiceProvider
	{
		void AppendSingleton(Type serviceType);
	}
}
