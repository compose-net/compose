namespace Compose
{
    public interface IDynamicRegister<T>
    {
		T CurrentService { get; }
		void Register(T instance);
    }
}
