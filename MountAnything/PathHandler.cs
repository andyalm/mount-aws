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

    public Item? GetItem()
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

    public IEnumerable<Item> GetChildItems(bool useCache = true)
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
        
        return Enumerable.Empty<Item>();
    }

    protected virtual bool ExistsImpl() => GetItem() != null;
    protected abstract Item? GetItemImpl();
    protected abstract IEnumerable<Item> GetChildItemsImpl();

    public virtual bool CacheChildren { get; } = true;
    
    public virtual IEnumerable<Item> GetChildItems(string filter)
    {
        var pathMatcher = new Regex("^" + Regex.Escape(ItemPath.Combine(Path, filter)).Replace(@"\*", ".*") + "$", RegexOptions.IgnoreCase);
        return GetChildItems(useCache: true)
            .Where(i => pathMatcher.IsMatch(i.FullPath));
    }
}