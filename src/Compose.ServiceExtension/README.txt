Compose.ServiceExtension
========================

Adding service extensions is very easy, but we hope it's been made even easier
by the addition of a "ServiceExtensions" class in your project.

Dependency Bindings
-------------------

In the provided code is a TODO comment in the GetDefaultServices method.  Add
your dependencies there, as you would a normal DI Container:

yield return describe.Transient<IYourService, YourServiceImplementation>();
yield return describe.Singleton<IYourSingleton, SingletonImplementation>();

These bindings can be overriden in configuration.  The ServiceDescriber will
look for a config key matching the fully qualified Interface name, and if it
finds one, will override the implementation specified in code with the fully
qualified service name in the config value.

Dependent Services
------------------

If your service depends on other Service Extensions, don't rely on your host
knowing that they needs to add your dependencies! Add them yourself within
the AddYourService method:

services.AddAnotherService();

Options Service
---------------

You can add support for the Microsoft Options Model extensions by running the 
following command in the Package Manager Console:

PM> Install-Package Microsoft.Framework.OptionsModel -Pre

As with adding in any other Service Extension, you then simple add the following
to the AddYourService method:

services.AddOptions();

Help / Issues
-------------

If you have any problems, please raise an issue on GitHub!

https://github.com/Smudge202/Compose/issues