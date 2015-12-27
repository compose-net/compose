# Compose

[![Join the chat at https://gitter.im/compose-net/compose](https://badges.gitter.im/compose-net/compose.svg)](https://gitter.im/compose-net/compose?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

[![Build status](https://img.shields.io/appveyor/ci/smudge202/compose-v3nsp.svg?style=plastic)](https://ci.appveyor.com/project/Smudge202/compose)
[![Latest Release](https://img.shields.io/nuget/vpre/compose.svg?style=plastic)](https://www.nuget.org/packages/compose)
[![NuGet](https://img.shields.io/nuget/dt/Compose.svg?style=plastic)](https://www.nuget.org/packages/compose)


[![Stories in Backlog](https://img.shields.io/github/issues-raw/compose-net/compose.svg?style=plastic)](http://waffle.io/compose-net/compose) [![Stories in Ready](https://badge.waffle.io/compose-net/compose.png?label=ready&title=Ready)](http://waffle.io/compose-net/compose) [![Stories in Progress](https://badge.waffle.io/compose-net/compose.png?label=in%20progress&title=In%20Progress)](http://waffle.io/compose-net/compose)

## <a name="description"></a>Description

_Compose_ is a lightweight framework to assist in application composition, enforcing clean and efficient component isolation, whilst providing key [features](#features) out of the box.

_Compose_ follows the same [OWIN] composition patterns as found in the [MVC 6 samples], but on a broader basis, minimising dependencies and providing consistent patterns across all application types, from ASP.Net MVC, to Console Applications.

## <a name="getting-started"></a>Getting Started

### Prerequisites

- .Net 4.5.1 or higher

### Install _Compose_
Includes built-in support for [Dependency Injection](#di), [Transitioning](#transitioning), [Snapshotting](#snapshotting), and an inheritable Executable Application.

To install _Compose_, run the following command in the [Package Manager Console]

```PS
PM> Install-Package Compose -Pre
```

### Basic Usage

## <a name="features"></a>Features

### <a name="di"></a>Dependency Injection

There are a myriad of DI Containers available nowadays such as [Autofac], [Castle Windsor], [Ninject], [StructureMap], [Unity], etc.

As part of [ASP.Net vNext] development, Microsoft are releasing an [Open Source Dependency Injection Framework] that supports all of the popular existing containers listed above.

We utilise this new framework so that, unless you need additional features provided by _Compose_, your services only require a dependency on the lightweight [Microsoft.Extensions.DependencyInjection.Abstractions] package.

### <a name="transitioning"></a>Transitioning

One of the key features _Compose_ provides is the ability to seamlessly switch your service implementations at runtime.  No additional code or changes are required in your service extensions, simply tag the service(s) as Transitional during composition.

For example:

```C#
app.UseServices(services => services .AddYourServices());
var alternativeProvider = app.UseProvider<YourServiceInterface>(services => services.AddAlternativeProvider());
// react to an event such as your database going offline
app.Transition(alternativeProvider);
```

### <a name="snapshotting"></a>Snapshotting

This feature allows your application to better manage dependencies, particularly those for which your application is not responsible.  For example, you may transition an interface from a service extension without any knowledge of how the interface is implemented.  Snapshotting allows you to _Rollback_ the dependency graph to use the original implementation, without the need to reference it directly.

```C#
app.UseServices(services => services.AddSomeServices());

// app.Snapshot() is called for you by UseServices, but you can
// call it at any stage to overwrite the existing snapshot.

var alternativeProvider = app.UseProvider<YourServiceInterface>(services => services.AddAlternativeProvider());
// Transition a component to your own implementation
app.Transition(alternativeProvider);
// When you want to transition back to the original service
app.Restore(); 
```

  [OWIN]: http://owin.org/
  [MVC 6 samples]: https://github.com/aspnet/Mvc/blob/dev/samples/MvcSandbox/Startup.cs
  [Autofac]: http://autofac.org
  [Ninject]: http://www.ninject.org
  [StructureMap]: http://structuremap.net
  [Unity]: http://unity.codeplex.com
  [Castle Windsor]: http://docs.castleproject.org/Windsor.MainPage.ashx
  [ASP.Net vNext]: http://www.asp.net/vnext
  [Open Source Dependency Injection Framework]: https://github.com/aspnet/DependencyInjection
  [Microsoft.Extensions.DependencyInjection.Abstractions]: http://www.nuget.org/packages/microsoft.extensions.dependencyinjection.abstractions
  [Package Manager Console]: http://docs.nuget.org/consume/package-manager-console
