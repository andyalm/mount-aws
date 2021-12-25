namespace MountAnything;

public class Cache
{
    private readonly Dictionary<string, CachedItem> _objects = new(StringComparer.OrdinalIgnoreCase);

    public void SetItem(IItem item)
    {
        foreach (var path in item.CacheablePaths)
        {
            if(_objects.TryGetValue(path, out var cachedItem))
            {
                cachedItem.Item = item;
            }
            else
            {
                _objects[path] = new CachedItem(item);
            }
        }

        foreach (var linkedItem in item.Links.Values)
        {
            SetItem(linkedItem);
        }
    }

    public bool TryGetItem(string path, out IItem cachedObject)
    {
        if (_objects.TryGetValue(path, out var cachedItem))
        {
            cachedObject = cachedItem.Item;
            return true;
        }

        cachedObject = default!;
        return false;
    }

    public bool TryGetItem<T>(string path, out T cachedItem) where T : IItem
    {
        if (TryGetItem(path, out var untypedCachedItem) && untypedCachedItem is T cachedTypedObject)
        {
            cachedItem = cachedTypedObject;
            return true;
        }

        cachedItem = default!;
        return false;
    }
    
    public void SetChildItems(IItem item, IEnumerable<IItem> childItems)
    {
        SetItem(item);
        foreach (var childItem in childItems)
        {
            SetItem(childItem);
        }
        var cachedItem = _objects[item.FullPath];
        cachedItem.ChildPaths = childItems.Select(i => i.FullPath).ToList();
    }

    public bool TryGetChildItems(string path, out IEnumerable<IItem> childItems)
    {
        if (_objects.TryGetValue(path, out var cachedItem) && cachedItem.ChildPaths != null)
        {
            childItems = cachedItem.ChildPaths.Select(childPath => _objects[childPath].Item).ToArray();
            return true;
        }

        childItems = default!;
        return false;
    }
    
    private class CachedItem
    {
        public CachedItem(IItem item)
        {
            Item = item;
            ChildPaths = null;
        }
        
        public IItem Item { get; set; }
        public List<string>? ChildPaths { get; set; }
    }
}