using System;

namespace Compose
{
	internal sealed class DelegateAbstractFactory<Service> : AbstractFactory<Service>
	{
		private readonly Func<Service> _delegateFactory;

		public DelegateAbstractFactory(Func<object> delegateFactory)
			: this(() => (Service) delegateFactory()) { }

		public DelegateAbstractFactory(Func<Service> delegateFactory)
		{
			_delegateFactory = delegateFactory;
		}

		public Service Create()
			=> _delegateFactory();
	}
}
