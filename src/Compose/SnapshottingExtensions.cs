using Microsoft.Extensions.DependencyInjection;

namespace Compose
{
	public static class SnapshottingExtensions
	{
		public static void Snapshot(this Application app)
			=> app.ApplicationServices?.GetService<TransitionManagerContainer>()?.Snapshot();

		public static void Restore(this Application app)
			=> app.ApplicationServices?.GetService<TransitionManagerContainer>()?.Restore();
	}
}
