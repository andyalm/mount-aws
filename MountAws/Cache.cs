namespace MountAws;

public class Cache
{
    private readonly Dictionary<string, CachedItem> _objects = new(StringComparer.OrdinalIgnoreCase);

    public void SetItem(AwsItem item)
    {
        if(_objects.TryGetValue(item.FullPath, out var cachedItem))
        {
            cachedItem.Item = item;
        }
        else
        {
            _objects[item.FullPath] = new CachedItem(item);
        }
    }

    public bool TryGetItem(string path, out AwsItem cachedObject)
    {
        if (_objects.TryGetValue(path, out var cachedItem))
        {
            cachedObject = cachedItem.Item;
            return true;
        }

        cachedObject = default!;
        return false;
    }

    public bool TryGetItem<T>(string path, out T cachedItem) where T : AwsItem
    {
        if (TryGetItem(path, out var untypedCachedItem) && untypedCachedItem is T cachedTypedObject)
        {
            cachedItem = cachedTypedObject;
            return true;
        }

        cachedItem = default!;
        return false;
    }
    
    public void SetChildItems(AwsItem item, IEnumerable<AwsItem> childItems)
    {
        SetItem(item);
        foreach (var childItem in childItems)
        {
            SetItem(childItem);
        }
        var cachedItem = _objects[item.FullPath];
        cachedItem.ChildPaths = childItems.Select(i => i.FullPath).ToList();
    }

    public bool TryGetChildItems(string path, out IEnumerable<AwsItem> childItems)
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
        public CachedItem(AwsItem item)
        {
            Item = item;
            ChildPaths = null;
        }
        
        public AwsItem Item { get; set; }
        public List<string>? ChildPaths { get; set; }
    }
}