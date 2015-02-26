namespace Compose
{
	public abstract class DirectTransition<TService, TDefault> : DefaultTransition<TService, TDefault>, ITransition<TService> where TDefault : TService
	{
		public DirectTransition(TService service) : base(service) { }

		public override bool Change<TImplementation>(TImplementation service)
		{
			Service = service;
			return true;
		}
	}
}
