namespace Compose
{
    public interface ITransitionManagerContainer
	{
		void Add<T>(IDynamicRegister<T> register);
        void Snapshot();
		void Restore();
	}
}
