using System.Management.Automation;
using System.Management.Automation.Provider;
using Autofac.Core;
using MountAnything.Routing;

namespace MountAnything;
public abstract class MountAnythingProvider : NavigationCmdletProvider, IPathHandlerContext
{
    private static readonly Cache _cache = new();
    private static Router? _router;
    protected override bool IsValidPath(string path) => true;
    public abstract Router CreateRouter();

    private Router Router
    {
        get
        {
            if (_router == null)
            {
                lock (GetType())
                {
                    if (_router == null)
                    {
                        _router = CreateRouter();
                    }
                }
            }

            return _router;
        }
    }

    public Cache Cache => _cache;
    
    bool IPathHandlerContext.Force => base.Force.IsPresent;
  
    protected override bool ItemExists(string path)
    {
        //WriteDebug($"ItemExists({path})");
        if (path.Contains("*"))
        {
            return false;
        }

        try
        {
            return WithPathHandler(path, handler => handler.Exists());
        }
        catch (Exception ex)
        {
            WriteDebug(ex.ToString());
            throw;
        }
    }

    protected override void GetItem(string path)
    {
        WriteDebug($"GetItem({path})");
        try
        {
            WithPathHandler(path, handler =>
            {
                var item = handler.GetItem();
                if (item != null)
                {
                    WriteItem(item);
                }
            });
        }
        catch (Exception ex)
        {
            WriteDebug(ex.ToString());
            throw;
        }
    }

    protected override void GetChildItems(string path, bool recurse)
    {
        WriteDebug($"GetChildItems({path}, {recurse})");
        WithPathHandler(path, handler =>
        {
            if (handler is IGetChildItemParameters handlerWithParams)
            {
                handlerWithParams.GetChildItemParameters = DynamicParameters;
            }
            var childItems = handler.GetChildItems(useCache: false);
            WriteItems(childItems);
        });
    }

    protected override object? GetChildItemsDynamicParameters(string path, bool recurse)
    {
        return WithPathHandler(path, handler =>
        {
            if (handler is IGetChildItemParameters handlerWithParams)
            {
                return handlerWithParams.GetChildItemParameters;
            }

            return null;
        });
    }

    protected override void GetChildItems(string path, bool recurse, uint depth)
    {
        GetChildItems(path, recurse);
    }
    
    protected override bool HasChildItems(string path)
    {
        WriteDebug($"HasChildItems({path})");
        return WithPathHandler<bool?>(path, handler => handler.GetChildItems(useCache:true).Any()) ?? false;
    }

    protected override bool IsItemContainer(string path)
    {
        WriteDebug($"IsItemContainer({path})");
        return WithPathHandler(path, handler => handler.GetItem()?.IsContainer) ?? false;
    }

    private void WriteItems<T>(IEnumerable<T> awsItems) where T : Item
    {
        foreach (var awsItem in awsItems)
        {
            WriteItem(awsItem);
        }
    }
    
    private void WriteItem(Item awsItem)
    {
        var providerPath = ToProviderPath(awsItem.FullPath);
        WriteDebug($"WriteItemObject<{awsItem.TypeName}>(,{providerPath},{awsItem.IsContainer})");
        WriteItemObject(awsItem.ToPipelineObject(), providerPath, awsItem.IsContainer);
    }

    private TReturn? WithPathHandler<TReturn>(string path, Func<IPathHandler,TReturn> action)
    {
        path = ItemPath.Normalize(path);
        RouteMatch? match = null;
        try
        {
            return Router.RouteToHandler(path, this, action);
        }
        catch (RoutingException ex)
        {
            WriteDebug($"RoutingException: {ex.Message}");
            return default;
        }
        catch (DependencyResolutionException ex) when(match != null)
        {
            WriteDebug($"Error creating handler {match.HandlerType.Name} with path {path}");
            WriteDebug(ex.ToString());
            WriteError(new ErrorRecord(ex, "2", ErrorCategory.NotSpecified, this));
            throw;
        }
        catch (Exception ex)
        {
            WriteDebug(ex.ToString());
            WriteError(new ErrorRecord(ex, "2", ErrorCategory.NotSpecified, this));
            throw;
        }
    }
    
    private void WithPathHandler(string path, Action<IPathHandler> action)
    {
        WithPathHandler<object?>(path, handler =>
        {
            action(handler);

            return null;
        });
    }
    
    public string ToProviderPath(string path)
    {
        return $"{ItemSeparator}{path.Replace("/", ItemSeparator.ToString())}";
    }

    protected override void GetChildNames(string path, ReturnContainers returnContainers)
    {
        WriteDebug($"GetChildNames({path}, {returnContainers})");
        base.GetChildNames(path, returnContainers);
    }

    protected override string[] ExpandPath(string path)
    {
        WriteDebug($"ExpandPath({path})");
        var normalizedPath = ItemPath.Normalize(path);
        var handlerPath = ItemPath.GetParent(normalizedPath);
        var pattern = ItemPath.GetLeaf(normalizedPath);
        var returnValue = WithPathHandler(handlerPath, handler =>
        {
            WriteDebug($"{handler.GetType().Name}.ExpandPath({pattern})");
            return handler.ExpandPath(pattern)
                .Select(ToProviderPath)
                .Select(p => ItemPath.GetParent(p) == "/" && p.StartsWith("/") ? p.Substring(1) : p)
                .ToArray();
        }) ?? Array.Empty<string>();
        foreach (var expandedPath in returnValue)
        {
            WriteDebug($"  {expandedPath}");
        }

        return returnValue;
    }

    protected override bool ConvertPath(string path, string filter, ref string updatedPath, ref string updatedFilter)
    {
        WriteDebug($"ConvertPath({path}, {filter})");
        return base.ConvertPath(path, filter, ref updatedPath, ref updatedFilter);
    }

    public override string GetResourceString(string baseName, string resourceId)
    {
        WriteDebug($"GetResourceString({baseName}, {resourceId})");
        return base.GetResourceString(baseName, resourceId);
    }

    protected override string NormalizeRelativePath(string path, string basePath)
    {
        var returnValue = base.NormalizeRelativePath(path, basePath);
        //HACK to make tab completion on top level directories work (I'm calling it a hack because I don't understand why its necessary)
        if (returnValue.StartsWith(ItemSeparator) && basePath == ItemSeparator.ToString())
        {
            returnValue = returnValue.Substring(1);
        }

        WriteDebug($"{returnValue} NormalizeRelativePath({path}, {basePath})");

        return returnValue;
    }
}
