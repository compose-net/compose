namespace Compose
{
	public class DirectFactoryTransition<TService, TDefault> : IFactoryTransition<TService>, ITransition<TService> where TDefault : TService
	{
		internal TService Service { get; set; }

		public DirectFactoryTransition(TDefault service) { Service = service; }

		public TService GetService() { return Service; }

		public virtual bool Change(TService service)
		{
			Service = service;
			return true;
		}
	}
}
