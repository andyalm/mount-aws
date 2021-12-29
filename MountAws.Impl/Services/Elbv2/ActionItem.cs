using System.Management.Automation;
using MountAnything;
using MountAws.Api.Elbv2;

namespace MountAws.Services.Elbv2;

public abstract class ActionItem : AwsItem
{
    public static ActionItem Create(string parentPath, PSObject action)
    {
        var actionType = action.Property<PSObject>("Type")!.Property<string>("Value")!;
        return actionType switch
        {
            "forward" => CreateForwardAction(parentPath, action),
            "redirect" => new RedirectActionItem(parentPath, action),
            "fixed-response" => new FixedActionItem(parentPath, action),
            _ => new DefaultActionItem(parentPath, action)
        };
    }

    private static ActionItem CreateForwardAction(string parentPath, PSObject action)
    {
        if (action.Property<PSObject>("ForwardConfig")?.Property<PSObject>("TargetGroups")?.Property<int>("Count") > 1)
        {
            return new WeightedForwardActionItem(parentPath, action);
        }
        else
        {
            return new ForwardActionItem(parentPath, action);
        }
    }
    
    public override string TypeName => typeof(ActionItem).FullName!;

    protected ActionItem(string parentPath, PSObject action) : base(parentPath, action)
    {
        ItemName = action.Property<PSObject>("Type")!.Property<string>("Value")!;
    }
    
    public override string ItemName { get; }
    
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