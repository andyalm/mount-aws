using MountAnything;

namespace MountAws;

public abstract class HierarchicalItemFetcher<TModel,TItem> where TItem : IItem
{
    protected abstract TItem CreateDirectoryItem(ItemPath parentPath, ItemPath directoryPath);
    protected abstract TItem CreateItem(ItemPath parentPath, TModel model);
    protected abstract ItemPath GetPath(TModel model);
    protected abstract IEnumerable<TModel> ListItems(ItemPath? pathPrefix);

    public IEnumerable<TItem> ListChildItems(ItemPath parentPath, ItemPath? pathPrefix = null)
    {
        var allModels = ListItems(pathPrefix).ToArray();
        var directories = Directories(allModels, pathPrefix);
        var childRoles = Children(allModels, pathPrefix);

        return directories.OrderBy(d => d.ToString())
            .Select(directory => CreateDirectoryItem(parentPath, directory))
            .Concat(childRoles.Select(m => CreateItem(parentPath, m)).OrderBy(i => i.ItemName));
    }
    
    private IEnumerable<ItemPath> Directories(IEnumerable<TModel> objects, ItemPath? pathPrefix)
    {
        return (from obj in objects
            let modelPath = GetPath(obj).Parent
            where !modelPath.IsRoot && modelPath.Parent.ToString().Equals(pathPrefix?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase)
            select modelPath).Distinct(new ItemPathEqualityComparer());
    }
    
    private IEnumerable<TModel> Children(IEnumerable<TModel> models, ItemPath? pathPrefix)
    {
        return from model in models
            let modelPath = GetPath(model)
            where modelPath.Parent.FullName.Equals(pathPrefix?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase)
            select model;
    }
    
    //TODO: Fix ItemPath equality in MountAnything
    private class ItemPathEqualityComparer : IEqualityComparer<ItemPath>
    {
        public bool Equals(ItemPath? x, ItemPath? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.FullName == y.FullName;
        }

        public int GetHashCode(ItemPath obj)
        {
            return obj.FullName.GetHashCode();
        }
    }
}