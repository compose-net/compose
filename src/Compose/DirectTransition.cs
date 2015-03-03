namespace Compose
{
	public class DirectTransition<TService> : ITransition<TService>
	{
		protected TService Service { get; private set; }

		public DirectTransition(TService service) { Service = service; }

		public bool Change<TImplementation>(TImplementation implementation) where TImplementation : TService
		{
			Service = implementation;
			return true;
		}
	}
}
