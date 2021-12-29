using System.Collections.Immutable;
using System.Management.Automation;

namespace MountAnything;

public abstract class Item<T> : IItem where T : class
{
    protected Item(string parentPath, T underlyingObject)
    {
        ParentPath = parentPath;
        UnderlyingObject = underlyingObject;
    }
    
    public string ParentPath { get; }

    public string FullPath => ItemPath.Combine(ParentPath, ItemName);
    public abstract string ItemName { get; }
    public abstract string ItemType { get; }
    public T UnderlyingObject { get; }
    public abstract bool IsContainer { get; }

    public virtual string TypeName => UnderlyingObject.GetType().FullName!;
    
    public virtual IEnumerable<string> Aliases => Enumerable.Empty<string>();

    public IEnumerable<string> CacheablePaths
    {
        get
        {
            yield return FullPath;
            foreach (var alias in Aliases)
            {
                yield return ItemPath.Combine(ParentPath, alias);
            }
        }
    }

    public virtual void CustomizePSObject(PSObject psObject) {}

    public PSObject ToPipelineObject(Func<string,string> pathResolver)
    {
        var psObject = UnderlyingObject is PSObject underlyingObject ? underlyingObject : new PSObject(UnderlyingObject);
        psObject.TypeNames.Clear(); 
        psObject.TypeNames.Add(TypeName);
        var itemNameProperty = psObject.Properties["ItemName"];
        if (itemNameProperty == null)
        {
            psObject.Properties.Add(new PSNoteProperty("ItemName", ItemName));
        }

        var nameProperty = psObject.Properties["Name"];
        if (nameProperty == null)
        {
            psObject.Properties.Add(new PSNoteProperty("Name", ItemName));
        }
        psObject.Properties.Add(new PSNoteProperty("ItemType", ItemType));
        foreach (var link in Links)
        {
            psObject.Properties.Add(new PSNoteProperty(link.Key, link.Value.ToPipelineObject(pathResolver)));
        }

        var linkObject = new PSObject();
        foreach (var link in Links)
        {
            linkObject.Properties.Add(new PSNoteProperty(link.Key, pathResolver(link.Value.FullPath)));
        }

        foreach (var linkPath in LinkPaths)
        {
            linkObject.Properties.Add(new PSNoteProperty(linkPath.Key, pathResolver(linkPath.Value)));
        }
        psObject.Properties.Add(new PSNoteProperty(nameof(Links), linkObject));
        CustomizePSObject(psObject);

        return psObject;
    }
    
    public ImmutableDictionary<string,Item> Links { get; protected init; } = ImmutableDictionary<string, Item>.Empty;
    public ImmutableDictionary<string,string> LinkPaths { get; protected init; } = ImmutableDictionary<string, string>.Empty;
}

public abstract class Item : Item<PSObject>
{
    protected Item(string parentPath, PSObject underlyingObject) : base(parentPath, underlyingObject)
    {
    }

    public override string ItemType => UnderlyingObject.TypeNames.First();
    
    protected T? Property<T>(string name)
    {
        return UnderlyingObject.Property<T?>(name);
    }
}