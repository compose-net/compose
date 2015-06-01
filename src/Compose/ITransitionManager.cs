using System;

namespace Compose
{
    public interface ITransitionManager<T>
    {
		void Snapshot();

		void Restore();

		void Change(Func<T> service);
    }
}
