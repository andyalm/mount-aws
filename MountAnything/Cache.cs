namespace MountAnything;

public class Cache
{
    private readonly Dictionary<string, CachedItem> _objects = new(StringComparer.OrdinalIgnoreCase);

    public void SetItem(IItem item)
    {
        foreach (var path in item.CacheablePaths)
        {
            if(_objects.TryGetValue(path.FullName, out var cachedItem))
            {
                cachedItem.Item = item;
            }
            else
            {
                _objects[path.FullName] = new CachedItem(item);
            }
        }

        foreach (var linkedItem in item.Links.Values)
        {
            SetItem(linkedItem);
        }
    }

    public bool TryGetItem(ItemPath path, out (IItem Item, DateTimeOffset FreshnessTimestamp) cachedObject)
    {
        if (_objects.TryGetValue(path.FullName, out var cachedItem))
        {
            cachedObject = (cachedItem.Item, cachedItem.FreshnessTimestamp);
            return true;
        }

        cachedObject = default!;
        return false;
    }
    
    public void SetChildItems(IItem item, IEnumerable<IItem> childItems)
    {
        SetItem(item);
        foreach (var childItem in childItems)
        {
            SetItem(childItem);
        }
        var cachedItem = _objects[item.FullPath.FullName];
        cachedItem.ChildPaths = childItems.Select(i => i.FullPath).ToList();
    }

    public bool TryGetChildItems(ItemPath path, out (IEnumerable<IItem> ChildItems, DateTimeOffset FreshnessTimestamp) cachedObject)
    {
        if (_objects.TryGetValue(path.FullName, out var cachedItem) && cachedItem.ChildPaths != null)
        {
            var childItems = cachedItem.ChildPaths.Select(childPath => _objects[childPath.FullName].Item).ToArray();
            cachedObject = (childItems, cachedItem.FreshnessTimestamp);
            return true;
        }

        cachedObject = default!;
        return false;
    }
    
    private class CachedItem
    {
        private IItem _item;

        public CachedItem(IItem item)
        {
            _item = item;
            FreshnessTimestamp = DateTimeOffset.UtcNow;
            ChildPaths = null;
        }

        public IItem Item
        {
            get => _item;
            set
            {
                _item = value;
                FreshnessTimestamp = DateTimeOffset.UtcNow;
            }
        }
        public List<ItemPath>? ChildPaths { get; set; }
        public DateTimeOffset FreshnessTimestamp { get; private set; }
    }
}