#Compose

[![Build status](https://ci.appveyor.com/api/projects/status/2a52f9nlucas0jsn?svg=true)](https://ci.appveyor.com/project/Smudge202/compose)

[TOC]

##<a name="description"></a>Description

_Compose_ is a lightweight framework to assist in application composition, enforcing clean and efficient component isolation, whilst providing key [features](#features) out of the box.

_Compose_ follows the same [OWIN] composition patterns as found in the [MVC 6 samples], but on a broader basis, minimising dependencies and providing consistent patterns across all application types, from ASP.Net MVC, to Console Applications.

##<a name="getting-started"></a>Getting Started

###Prerequisites

- .Net 4.5.2 or higher

###Install _Compose_
Includes built-in support for [Dependency Injection](#di), [Transitioning](#transitioning), [Snapshotting](#snapshotting), and basic Executable Applications.

To install _Compose_, run the following command in the [Package Manager Console]

```PS
PM> Install-Package Compose -Pre
```

###Basic Usage

##<a name="features"></a>Features

###<a name="di"></a>Dependency Injection

There are a myriad of DI Containers available nowadays such as [Autofac], [Castle Windsor], [Ninject], [StructureMap], [Unity], etc.

As part of [ASP.Net vNext] development, Microsoft are releasing an [Open Source Dependency Injection Framework] that supports all of the popular existing containers listed above.

We utilise this new framework so that, unless you need additional features provided by _Compose_, your services only require a dependency on the [Microsoft.Framework.DependencyInjection package].

###<a name="transitioning"></a>Transitioning

One of the key features _Compose_ provides is the ability to seamlessly switch your service implementations at runtime.  No additional code or changes are required in your service extensions, simply tag the service(s) as Transitional during composition.

For example:

```C#
app.UseServices(services => 
{
	services
		.AddYourServices()
		// Mark one of the interfaces added by your service as transitional
		.WithTransitional<IDependency>();
});
// So long as an interface is regsitered as transitional, you can change it at any stage using
app.Transition<IDependency, MyReplacementDependency>();
```

###<a name="snapshotting"></a>Snapshotting

This feature allows your application to better manage dependencies, particularly those for which your application is not responsible.  For example, you may transition an interface from a service extension without any knowledge of how the interface is implemented.  Snapshotting allows you to _Rollback_ the dependency graph to use the original implementation, without the need to reference it directly.

```C#
app.UseServices(services =>
{
	services
		.AddSomeServices()
		// Mark all interfaces as transitional
		.AsTransitional();
});
// app.Snapshot() is called for you by UseServices, but you can
// call it at any stage to overwrite the existing snapshot.

// Transition a component to your own implementation
app.Transition<IDependency, MyReplacementDependency>();
// When you want to transition back to the original service
app.Restore(); 
```

##FAQ / Troubleshooting

-Q. How to debug/view the code emitted?
-A. Add the `ENABLE_SAVE_DYNAMIC_ASSEMBLY` compilation symbol to the _Compose_ build to have the dyanmic assemblies stored in the execution directory.

  [OWIN]: http://owin.org/
  [MVC 6 samples]: https://github.com/aspnet/Mvc/blob/dev/samples/MvcSample.Web/Startup.cs
  [Autofac]: http://autofac.org
  [Ninject]: http://www.ninject.org
  [StructureMap]: http://structuremap.net
  [Unity]: http://unity.codeplex.com
  [Castle Windsor]: http://docs.castleproject.org/Windsor.MainPage.ashx
  [ASP.Net vNext]: http://www.asp.net/vnext
  [Open Source Dependency Injection Framework]: https://github.com/aspnet/DependencyInjection
  [Microsoft.Framework.DependencyInjection package]: http://www.nuget.org/packages/microsoft.framework.dependencyinjection
  [Package Manager Console]: http://docs.nuget.org/consume/package-manager-console
