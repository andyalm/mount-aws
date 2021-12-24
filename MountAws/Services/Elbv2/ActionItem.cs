using System.Management.Automation;
using MountAnything;
using MountAws.Api.Elbv2;

namespace MountAws.Services.Elbv2;

public abstract class ActionItem : Item
{
    public static ActionItem Create(string parentPath, PSObject action)
    {
        var actionType = action.Property<string>("Type");
        if (actionType == "Forward" && 
            action.Property<PSObject>("ForwardConfig")?.Property<PSObject>("TargetGroups")?.Property<int>("Count") > 1)
        {
            return new WeightedForwardActionItem(parentPath, action);
        }
        if (actionType == "Forward")
        {
            return new ForwardActionItem(parentPath, action);
        }
        if (actionType == "Redirect")
        {
            return new RedirectActionItem(parentPath, action);
        }
        if (actionType == "FixedResponse")
        {
            return new FixedActionItem(parentPath, action);
        }

        return new DefaultActionItem(parentPath, action);
    }
    
    public override string TypeName => "MountAws.Services.ELBV2.ActionItem";

    protected ActionItem(string parentPath, PSObject action) : base(parentPath, action) {}
    public abstract string Description { get; }
    public override void CustomizePSObject(PSObject psObject)
    {
        psObject.Properties.Add(new PSNoteProperty("Description", Description));
    }

    public virtual IEnumerable<Item> GetChildren(IElbv2Api elbv2)
    {
        return Enumerable.Empty<Item>();
    }
}