namespace Compose
{
	public class DirectTransition<TService> : ITransition<TService>
	{
		protected TService Service { get; private set; }

		public DirectTransition(TService service) { Service = service; }

		public bool Change(TService implementation)
		{
			Service = implementation;
			return true;
		}
	}
}
