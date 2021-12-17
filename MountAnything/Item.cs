using System.Collections.Immutable;
using System.Management.Automation;

namespace MountAnything;

public abstract class Item
{
    protected Item(string parentPath)
    {
        ParentPath = parentPath;
    }
    
    public string ParentPath { get; }

    public string FullPath => ItemPath.Combine(ParentPath, ItemName);
    public abstract string ItemName { get; }
    public abstract object UnderlyingObject { get; }
    public abstract string ItemType { get; }
    
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
        var psObject = new PSObject(UnderlyingObject);
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
        psObject.Properties.Add(new PSNoteProperty(nameof(Links), linkObject));
        CustomizePSObject(psObject);

        return psObject;
    }
    
    public ImmutableDictionary<string,Item> Links { get; protected init; } = ImmutableDictionary<string, Item>.Empty;
}