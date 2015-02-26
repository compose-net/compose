namespace Compose
{
	public abstract class DirectTransition<TService, TDefault> 
		: DefaultDirectTransition<TService, TDefault> where TDefault : TService
	{
		public DirectTransition(TService service) : base(service) { }

		public override bool Change<TImplementation>(TImplementation service)
		{
			Service = service;
			return true;
		}
	}
}
