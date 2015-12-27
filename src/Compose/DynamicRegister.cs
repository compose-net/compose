namespace Compose
{
	public interface DynamicRegister<Service>
	{
		Service CurrentService { get; set; }
		Service SnapshotService { get; set; }
		void Register(Service instance);
	}
}
