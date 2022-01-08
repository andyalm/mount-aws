using System.Management.Automation;
using Amazon.WAFV2;
using Amazon.WAFV2.Model;
using MountAnything;
using MountAws.Services.Wafv2.StatementNavigation;

namespace MountAws.Services.Wafv2;

public class RuleItem : AwsItem<Rule>
{
    public RuleItem(ItemPath parentPath, Rule underlyingObject, IAmazonWAFV2 wafv2) : base(parentPath, underlyingObject)
    {
        ActionName = UnderlyingObject.ActionItem(FullPath)?.ActionName;
        if (ActionName == null)
        {
            var overrideAction = UnderlyingObject.OverrideActionItem(FullPath);
            ActionName = overrideAction?.ActionName == "none" ? "default" : overrideAction?.ActionName;
        }
        StatementDescription = UnderlyingObject.Statement.ToNavigator(wafv2).Description;
    }

    public string StatementDescription { get; }
    public override string ItemName => UnderlyingObject.Name;
    public override bool IsContainer => true;

    public string? ActionName { get; }
    
    protected override void CustomizePSObject(PSObject psObject)
    {
        base.CustomizePSObject(psObject);
        psObject.Properties.Add(new PSNoteProperty(nameof(ActionName), ActionName));
        psObject.Properties.Add(new PSNoteProperty(nameof(StatementDescription), StatementDescription));
    }
}