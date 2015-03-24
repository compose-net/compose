Compose.Owin
========================

Adapter to allow OWIN based web applications to easily utilise Compose.

Usage
-----

The simplest and most conventional way of including Compose based
services, is to instantiate an OwinApplication in your Startup:

``````````````````````````````````````````````````
[assembly: OwinStartup(typeof(Startup))]
public class Startup
{
	public void Configuration(IAppBuilder builder)
	{
		var app = new OwinApplication(builder);
		app.UseServices(services => {
			
			services.AddYourServices();

		});
		app.UseYourApplication();
	}
}
``````````````````````````````````````````````````

Application Extensions
----------------------

The following is an example of how your application can provide
an OWIN based application extension:

``````````````````````````````````````````````````
internal sealed class OwinExecutor
{
	private readonly IDependency _dependency;
	public OwinExecutor(IDependency dependency)
	{
		_dependency = dependency;
	}

	public Task Handle(IOwinContext context)
	{
		if (context == null || 
			context.Request == null || 
			context.Request.Body == null || 
			context.Request.Body.Length == 0)
                return Task.FromResult(false);

		var appResponse = _dependency.Handle(context.Request.Body);
		var webResponse = $"HTTP/1.1 200 OK\r\nContent-Length:{response.Length}\r\n{response}\r\n";
		return context.Response.WriteAsync(webResponse);
	}
}

public static class ApplicationExtensions
{
	public static void UseYourApplication(this OwinApplication app)
	{
		app.OnExecute<OwinExecutor>((executor, context) => executor.Handle(context));
	}
}
``````````````````````````````````````````````````

The OwinExecutor is responsible for validating and extracting any information
your application requires from the Web Request, and then writing out your
application's response.

You can then add a very simple Application Extension passing through
the OWIN context to your executor.

Help / Issues
-------------

If you have any problems, please raise an issue on GitHub!

https://github.com/Smudge202/Compose/issues