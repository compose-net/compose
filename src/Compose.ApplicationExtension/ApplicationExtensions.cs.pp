using Compose;
using Microsoft.Framework.DependencyInjection;

namespace $rootnamespace$
{
	public static class ApplicationExtensions
	{
		// TODO : remove some of the methods below and/or replace/constrain the generics to only
		// support applications of your choosing.
		public static void UseYourApplication(this Executable app)
		{
			// shows how to delegate out to a method in your application
			app.OnExecute(MyApplication.OnStart);
		}

		public static void UseYourApplication<TResult>(this Executable<TResult> app)
		{
			// shows how to perform a composition build up
			app.OnExecute<MyApplication>(myApp => myApp.Start<TResult>());
		}

		public static void UseYourApplication<TContext, TResult>(this Executable<TContext, TResult> app)
		{
			// shows how to pass through context
			app.OnExecute<MyApplication>((myApp, context) => myApp.Start<TContext, TResult>(context));
		}

		// TODO : Remove this (just here to remove compile errors in sample code above)
		#region Delete Me
		private class MyApplication
		{
			public static void OnStart() { }
			public TResult Start<TResult>() { return default(TResult); }
			public TResult Start<TContext, TResult>(TContext context) { return default(TResult); }
		}
		#endregion
	}
}
