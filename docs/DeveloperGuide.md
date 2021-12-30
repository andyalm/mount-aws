# Developer Guide

Here is an overview of the MountAws codebase to help you out if you would like to contribute.

## Key abstractions

There are three key abstractions that drive MountAws. The `Router`, `PathHandler`'s, and `AwsItem`'s:

### Router

Every path in the virtual filesystem is processed by the `Router` to determine which `PathHandler` will process the request.
The Router API composes a nested hierarchy of routes. Under the hood, routes are regex based, but you usually can use a more convenient
extension method to avoid needing to actually deal with regex's. Here is an example of the router api in action:

```c#
router.MapRegex<RegionHandler>("(?<Region>[a-z0-9-]+)", region =>
{
    region.MapLiteral<Ec2RootHandler>("ec2", ec2 =>
    {
        ec2.MapLiteral<InstancesHandler>("instances", instances =>
        {
            instances.Map<InstanceHandler>();
        });
        ec2.MapLiteral<SecurityGroupsHandler>("security-groups", securityGroups =>
        {
            securityGroups.Map<SecurityGroupHandler>();
        });
    });
});
```

In the example, you can see three different variations of `Map` methods used. All of them take a generic type argument that corresponds to the `IPathHandler` that will be invoked for matching routes. They are:

  * `MapLiteral` - This matches on the literal string (e.g. constant) passed into it. Only that literal string will match the route.
  * `Map` - This matches any supported character (pretty much anything besides a `/`, which is used as the path separator) at this hierarchy level. You can optionally pass in a string as the first argument to this method if you would like to capture the value of the matched value. The captured value will be given the name that is passed as the argument. The captured value can be used for dependency injection into the `PathHander` of this or any child route.
  * `MapRegex` - This is the lower level method that the above two methods call under the hood. Any regex is acceptable, so long as it does not contain the `^` or `$` characters for declaring the beginning or end of a string. Those are implicitly added by the router as necessary. It is important to note that any regex you are adding is implicitly concatenated with the regex's built by parent and child routes when the router is matching. Named captures are allowed in the regex and those captured values can  be used for dependency injection into the `PathHandler` of this or any child route.

### PathHandler

The `PathHandler` is in charge of processing a command to the powershell provider.
While there is an `IPathHandler`, it is expected that 99% of the time you will want to use 
the `PathHandler` abstract base class instead for convenience. It will automatically handle
things like caching for you, which helps make things like tab completion as performant as possible.

The `PathHandler` base class has only two methods that you are required to implement:

 * `GetItemImpl` - This is called when the `Get-Item` command is called. It should return the `IItem` that corresponds to the path that this `PathHandler` is processing. If no item exists at this path, it should return `null`.
 * `GetChildItemsImpl` - This is called when the `Get-ChildItems` command. Its also used to support tab completion by default. It should return all of the child items of the item returned by the `GetItemImpl` method.

In addition, you can optionally override the following methods when helpful/necessary:

 * `ExistsImpl` - By default, existence is checked by calling `GetItem` and determining if it returned `null` or not. However, if you can provide a more performant/optimal implementation, you can override this method.
 * `GetChildItems(string filter)` - This method supports tab completion, as well as when the `-Filter` argument is used on the `Get-ChildItems` command. By default, the `GetChildItemsImpl` method is called and the filter as applied to entire set of items returned. However, if you can provide a more performant implementation that does not require fetching all items first, you are encouraged to do so by overriding this method.
 * `CacheChildren` - By default, the paths of the child items returned by `GetChildItemsImpl` are cached to help make things like tab completion faster. However, if there are potentially a very large number of child items for this handler, you may want to tell it not to do this by overriding this property and returning `false`.
 * `GetItemCommandDefaultFreshness` - This allows you to customize when the cache is used for `Get-Item` commands.
 * `GetChildItemsCommandDefaultFreshness` - This allows you to customize when the cache is used for `Get-ChildItems` commands.

### AwsItem

This represents the object/item that is returned to the console by `Get-Item` and `Get-ChildItems` commands. It is generally a wrapper
class around an underlying object that will be sent to the console. There is a generic version of `AwsItem<T>` where the type
argument represents the .net type of the item that will be sent to the console. If you inherit from the non-generic `AwsItem`, the
underlying type will be a `PSObject`. Either way, all properties on the underlying type will be written to the powershell pipeline. The
`AwsItem` class has a couple methods that need to be implemented in the subclass to tell the powershell provider what the path of the item is:

 * `ItemName` - This identifies the virtual "filename" of the item. It should be something that naturally identifies the item. Prefer human friendly names if  they are guaranteed to be unique. Otherwise, use the natural aws identifier. Do *not* use an ARN though. They are too long and are not friendly to the paradigm of a virtual filesystem like this.
 * `IsContainer` - This indicates whether this could have child items or not.

Here is an example of a simple `AwsItem` implementation:

```c#
public class SecurityGroupItem : AwsItem<SecurityGroup>
{
    public SecurityGroupItem(string parentPath, SecurityGroup securityGroup) : base(parentPath, securityGroup) {}

    public override string ItemName => UnderlyingObject.GroupId;
    
    public override bool IsContainer => false;
}
```

## Adding support for a new service

The code for each aws service is located in the `MountAws.Impl` project under the `Services` folder. To add
support for a new service, create a folder for the new service. Then create a `Routes.cs` file which should contain an
extension method called `MapXXX` where `XXX` is the name of the aws service. It should contain the routes for that service
and the `MountAwsRouterFactory` should be updated to call that method. You will need to create a `XXXRootHandler` class
that inherits from `PathHandler` and will handle the virtual "directory" for that aws service. Its `GetChildItemsImpl` method
will return the items that are the top level items for that service. From there, create the `PathHandler` and `AwsItem` abstractions
for the entire hierarchy that you want to create. Finally, you can create a `Formats.ps1xml` file that contains customize `View` elements for
each item that you created. This tells powershell how to display the item in the console by default. You should choose the most useful
few properties of the object that you want displayed and the property that maps to the `ItemName` should be the first one.