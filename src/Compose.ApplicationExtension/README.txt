Compose.ApplicationExtension
============================

Adding application extensions is very easy, but we hope it's been made even 
easier by the addition of an "ApplicationExtensions" class in your project.

OnExecute
---------

The intention of an application extension is to specify what code should
be called when the application is executed. This is done through the
`OnExecute` method of the Executable(s) you choose to extend.

Dependency Injection
--------------------

Compose fully supports Dependency Injection out of the box. Simply specify
the type you want to build up as a generic on the `OnExecute` method.  

For example:

app.OnExecute<MyApplication>(app => app.DoSomeStuff());

Compose will create an instance of MyApplication, and inject any dependencies
you have configured into the constructor.

Help / Issues
-------------

If you have any problems, please raise an issue on GitHub!

https://github.com/Smudge202/Compose/issues