namespace Compose.Transitional
{
	public abstract class DirectTransition<T> : ITransition<T>
	{
		public T Service { get; protected set; }

		public virtual bool Change<TService>(TService service) where TService : T
		{
			Service = service;
			return true;
		}
	}
}
