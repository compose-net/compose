using System;

namespace Compose
{
	public class Executable : Application
	{
		protected Action Execution { get; private set; }

		public void OnExecute(Action invoke)
		{
			Execution = invoke;
		}

		public void OnExecute<TService>(Action<TService> invoke) where TService : class
		{
			OnExecute(() => invoke(GetRequiredService<TService>()));
		}

		public virtual void Execute()
		{
			Execution();
		}
	}

	public abstract class Executable<TResult> : Application
	{
		protected Func<TResult> Execution { get; private set; }

		public void OnExecute(Func<TResult> invoke)
		{
			Execution = invoke;
		}

		public void OnExecute<TService>(Func<TService, TResult> invoke) where TService : class
		{
			OnExecute(() => invoke(GetRequiredService<TService>()));
		}

		public virtual TResult Execute()
		{
			return Execution();
		}
	}

	public abstract class Executable<TContext, TResult> : Application
	{
		protected Func<TContext, TResult> Execution { get; private set; }

		public void OnExecute(Func<TContext, TResult> invoke)
		{
			Execution = invoke;
		}

		public void OnExecute<TService>(Func<TService, TContext, TResult> invoke) where TService : class
		{
			OnExecute(context => invoke(GetRequiredService<TService>(), context));
		}

		public virtual TResult Execute(TContext context)
		{
			return Execution(context);
		}
	}
}
