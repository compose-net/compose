using System;

namespace Compose
{
    public interface ITransitionManager<T>
    {
		void Change(Func<T> service);
    }
}
