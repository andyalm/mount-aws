using System.Collections;
using System.Management.Automation;
using System.Management.Automation.Provider;
using Autofac;
using Autofac.Core;
using MountAnything.Content;
using MountAnything.Routing;

namespace MountAnything;
public abstract class MountAnythingProvider : NavigationCmdletProvider,
    IPathHandlerContext,
    IContentCmdletProvider
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
            var childItems = string.IsNullOrEmpty(Filter)
                ? handler.GetChildItems(useCache: false)
                : handler.GetChildItems(Filter);
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

    protected override void RemoveItem(string path, bool recurse)
    {
        WithPathHandler(path, handler =>
        {
            if (handler is IRemoveItemHandler removeItemHandler)
            {
                removeItemHandler.RemoveItem();
            }
            else
            {
                throw new InvalidOperationException($"MountAws does not currently support removing this item");
            }
        });
    }

    private void WriteItems<T>(IEnumerable<T> items) where T : Item
    {
        foreach (var item in items)
        {
            WriteItem(item);
        }
    }
    
    private void WriteItem(Item item)
    {
        Cache.SetItem(item);
        var providerPath = ToProviderPath(item.FullPath);
        WriteDebug($"WriteItemObject<{item.TypeName}>(,{providerPath},{item.IsContainer})");
        WriteItemObject(item.ToPipelineObject(ToFullyQualifiedProviderPath), providerPath, item.IsContainer);
    }

    private (IPathHandler Handler, ILifetimeScope Container) GetPathHandler(string path)
    {
        path = ItemPath.Normalize(path);
        return Router.RouteToHandler(path, this);
    }

    private TReturn? WithPathHandler<TReturn>(string path, Func<IPathHandler,TReturn> action)
    {
        try
        {
            var (handler, container) = GetPathHandler(path);
            using (container)
            {
                return action(handler);
            }
        }
        catch (RoutingException ex)
        {
            WriteDebug($"RoutingException: {ex.Message}");
            return default;
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

    public string ToFullyQualifiedProviderPath(string path)
    {
        return $"{PSDriveInfo.Name}:{ToProviderPath(path)}";
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
            return handler.GetChildItems(pattern)
                .Select(i => ToProviderPath(i.FullPath))
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

    #region Content

    public void ClearContent(string path)
    {
        using var contentWriter = GetContentWriter(path);
        contentWriter.Write(new ArrayList());
        contentWriter.Close();
    }

    public object? ClearContentDynamicParameters(string path)
    {
        return null;
    }

    public IContentReader GetContentReader(string path)
    {
        var (handler, container) = GetPathHandler(path);
        if (handler is IContentReaderHandler contentReadHandler)
        {
            return new HandlerDisposingProxy(container, contentReadHandler.GetContentReader());
        }

        container.Dispose();
        throw new InvalidOperationException("This item does not support reading content");
    }

    public object? GetContentReaderDynamicParameters(string path)
    {
        return null;
    }

    public IContentWriter GetContentWriter(string path)
    {
        var (handler, container) = GetPathHandler(path);
        if (handler is IContentWriterHandler contentWriteHandler)
        {
            return new HandlerDisposingProxy(container, contentWriteHandler.GetContentWriter());
        }

        container.Dispose();
        throw new InvalidOperationException("This item does not support writing content");
    }

    public object? GetContentWriterDynamicParameters(string path)
    {
        return null;
    }

    #endregion
}
