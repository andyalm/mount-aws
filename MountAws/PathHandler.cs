using System.Management.Automation;

namespace MountAws;

public abstract class PathHandler : IPathHandler
{
    protected PathHandler(string path, IPathHandlerContext context)
    {
        Path = path;
        Context = context;
    }
    
    public string Path { get; }
    protected IPathHandlerContext Context { get; }

    protected Cache Cache => Context.Cache;
    protected void WriteDebug(string message) => Context.WriteDebug(message);

    protected string ParentPath => System.IO.Path.GetDirectoryName(Path)!.Replace(@"\", "/");
    protected string ItemName => System.IO.Path.GetFileName(Path);

    public bool Exists()
    {
        if (Cache.TryGetItem(Path, out _))
        {
            return true;
        }

        return ExistsImpl();
    }

    public AwsItem? GetItem()
    {
        if (!Context.Force && Cache.TryGetItem(Path, out var cachedItem))
        {
            return cachedItem;
        }

        var item = GetItemImpl();
        if (item != null)
        {
            WriteDebug($"Cache.SetItem<{item.GetType().Name}>({item.FullPath})");
            Cache.SetItem(item);
        }

        return item;
    }

    public IEnumerable<AwsItem> GetChildItems(bool useCache = false)
    {
        if (useCache && !Context.Force && Cache.TryGetChildItems(Path, out var cachedChildItems))
        {
            WriteDebug($"True Cache.TryGetChildItems({Path})");
            return cachedChildItems;
        }
        WriteDebug($"False Cache.TryGetChildItems({Path})");

        var item = GetItem();
        if (item != null)
        {
            var childItems = GetChildItemsImpl().ToArray();
            WriteDebug($"Cache.SetChildItems({item.FullPath}, {childItems.Length})");
            Cache.SetChildItems(item, childItems);

            return childItems;
        }
        
        return Enumerable.Empty<AwsItem>();
    }

    public virtual IEnumerable<AwsItem> NormalizeChildItems(IEnumerable<AwsItem> items)
    {
        return items;
    }

    protected abstract bool ExistsImpl();
    protected abstract AwsItem? GetItemImpl();
    protected abstract IEnumerable<AwsItem> GetChildItemsImpl();
}