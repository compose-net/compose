using System;

namespace Compose
{
	public interface TransitionManager<T>
	{
		void Change(Func<T> service);
	}
}
