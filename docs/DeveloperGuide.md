# Developer Guide

Here is an overview of the MountAws codebase to help you out if you would like to contribute. MountAws uses [MountAnything](https://github.com/andyalm/mount-anything), which
is a framework for building powershell providers. Start by familiarizing yourself with it by reading [it's docs](https://github.com/andyalm/mount-anything/blob/main/README.md)

The specific things to know about `MountAws`, is all items inherit from `AwsItem` or `AwsItem<T>`, which is a subclass of `Item` from `MountAnything`.
When setting the `ItemName` of an item, prefer human friendly names if they are guaranteed to be unique. Otherwise, use the natural aws identifier.
Do *not* use an ARN though. They are too long and are not friendly to the paradigm of a virtual filesystem like this.

## Adding support for a new service

The code for each aws service is located in the `MountAws.Impl` project under the `Services` folder. To add
support for a new service, create a folder for the new service. Then create a `IServiceRoutes` implementation which should contain a
method to build all of the routes, starting with a `.MapLiteral<XXXRootHandler>("xxx")`, where `XXX` is the name of the aws service.
It should contain the routes for that service. You will need to create a `XXXRootHandler` class
that inherits from `PathHandler` and will handle the virtual "directory" for that aws service. Its `GetChildItemsImpl` method
will return the items that are the top level items for that service. From there, create the `PathHandler` and `AwsItem` abstractions
for the entire hierarchy that you want to create. Finally, you can create a `Formats.ps1xml` file that contains customized `View` elements for
each item that you created. This tells powershell how to display the item in the console by default. You should choose the most useful
few properties of the object that you want displayed and the property that maps to the `ItemName` should be the first one.