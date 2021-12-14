namespace MountAnything;

public class Cache
{
    private readonly Dictionary<string, CachedItem> _objects = new(StringComparer.OrdinalIgnoreCase);

    public void SetItem(Item item)
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
    }

    public bool TryGetItem(string path, out Item cachedObject)
    {
        if (_objects.TryGetValue(path, out var cachedItem))
        {
            cachedObject = cachedItem.Item;
            return true;
        }

        cachedObject = default!;
        return false;
    }

    public bool TryGetItem<T>(string path, out T cachedItem) where T : Item
    {
        if (TryGetItem(path, out var untypedCachedItem) && untypedCachedItem is T cachedTypedObject)
        {
            cachedItem = cachedTypedObject;
            return true;
        }

        cachedItem = default!;
        return false;
    }
    
    public void SetChildItems(Item item, IEnumerable<Item> childItems)
    {
        SetItem(item);
        foreach (var childItem in childItems)
        {
            SetItem(childItem);
        }
        var cachedItem = _objects[item.FullPath];
        cachedItem.ChildPaths = childItems.Select(i => i.FullPath).ToList();
    }

    public bool TryGetChildItems(string path, out IEnumerable<Item> childItems)
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
        public CachedItem(Item item)
        {
            Item = item;
            ChildPaths = null;
        }
        
        public Item Item { get; set; }
        public List<string>? ChildPaths { get; set; }
    }
}