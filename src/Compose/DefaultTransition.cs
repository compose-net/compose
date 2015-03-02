namespace Compose
{
	public abstract class DefaultDirectTransition<TService, TDefault> 
		: IDirectTransition<TService> where TDefault : TService
	{
		public TService Service { get; protected set; }

		public DefaultDirectTransition(TService service) { Service = service; }

		public abstract bool Change<TImplementation>(TImplementation service) where TImplementation : TService;
	}
}
