using System.Management.Automation;

namespace MountAws;

public abstract class AwsItem
{
    public abstract string FullPath { get; }
    public abstract string Name { get; }
    public abstract object UnderlyingObject { get; }
    
    public abstract string ItemType { get; }
    
    public abstract bool IsContainer { get; }

    public virtual string TypeName => UnderlyingObject.GetType().FullName!;

    public virtual void CustomizePSObject(PSObject psObject) {}

    public PSObject ToPipelineObject()
    {
        var psObject = new PSObject(UnderlyingObject);
        psObject.TypeNames.Add(TypeName);
        psObject.Properties.Add(new PSNoteProperty("Name", Name));
        psObject.Properties.Add(new PSNoteProperty("ItemType", ItemType));
        CustomizePSObject(psObject);

        return psObject;
    }
}