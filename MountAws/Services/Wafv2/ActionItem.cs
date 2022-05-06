using System.Management.Automation;
using Amazon.WAFV2.Model;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Wafv2;

public class ActionItem : AwsItem
{
    public ActionItem(ItemPath parentPath, string itemName, object actionObject, string actionName) : base(parentPath, actionObject.ToPSObject())
    {
        ItemName = itemName;
        ActionObject = actionObject;
        ActionName = actionName;
    }

    public override string ItemName { get; }
    public string ActionName { get; }
    public object ActionObject { get; }
    protected override string TypeName => typeof(GenericContainerItem).FullName!;
    public override bool IsContainer => true;
    protected override void CustomizePSObject(PSObject psObject)
    {
        base.CustomizePSObject(psObject);
        psObject.Properties.Add(new PSNoteProperty(nameof(ActionName), ActionName));
        psObject.Properties.Add(new PSNoteProperty(nameof(GenericContainerItem.Description), $"{ActionName} - requests matching this rule are {ActionName}ed"));
    }
}