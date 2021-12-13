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

    public virtual void CustomizePSObject(PSObject psObject) {}

    public PSObject ToPipelineObject()
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
        CustomizePSObject(psObject);

        return psObject;
    }
}