namespace Compose
{
	public interface TransitionManagerContainer
	{
		void Add<T>(DynamicRegister<T> register);
		void Snapshot();
		void Restore();
	}
}
