namespace Compose.Transitional
{
	public interface ITransition<T>
	{
		T Service { get; }
		bool Change<TService>(TService service) where TService : T;
	}
}
