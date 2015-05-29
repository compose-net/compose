using System;

namespace Compose
{
    public interface ITransitionManager<T>
    {
		T CurrentService { get; }

		void Snapshot();

		void Restore();

		void Change(Func<T> service);
    }
}
