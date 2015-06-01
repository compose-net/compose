namespace Compose
{
    public interface IDynamicRegister<T>
    {
		T CurrentService { get; set; }
		T SnapshotService { get; set; }
		void Register(T instance);
    }
}
