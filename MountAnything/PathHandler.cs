using System.Management.Automation;
using System.Text.RegularExpressions;

namespace MountAnything;

public abstract class PathHandler : IPathHandler
{
    protected PathHandler(string path, IPathHandlerContext context)
    {
        Path = path;
        Context = context;
        LinkGenerator = new LinkGenerator(path);
    }
    
    public string Path { get; }
    protected IPathHandlerContext Context { get; }
    
    protected LinkGenerator LinkGenerator { get; }

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

    public IItem? GetItem()
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

    public IEnumerable<IItem> GetChildItems(bool useCache = false)
    {
        if (useCache && CacheChildren && !Context.Force && Cache.TryGetChildItems(Path, out var cachedChildItems))
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
            if (CacheChildren)
            {
                Cache.SetChildItems(item, childItems);
            }

            return childItems;
        }
        
        return Enumerable.Empty<IItem>();
    }

    protected virtual bool ExistsImpl() => GetItem() != null;
    protected abstract IItem? GetItemImpl();
    protected abstract IEnumerable<IItem> GetChildItemsImpl();

    public virtual bool CacheChildren => true;

    public virtual IEnumerable<IItem> GetChildItems(string filter)
    {
        var pathMatcher = new Regex("^" + Regex.Escape(ItemPath.Combine(Path, filter)).Replace(@"\*", ".*") + "$", RegexOptions.IgnoreCase);
        return GetChildItems(useCache: true)
            .Where(i => pathMatcher.IsMatch(i.FullPath));
    }
}