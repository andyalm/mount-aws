using System.Management.Automation;
using Amazon.ElasticLoadBalancingV2;
using MountAnything;
using Action = Amazon.ElasticLoadBalancingV2.Model.Action;

namespace MountAws.Services.Elbv2;

public abstract class ActionItem : AwsItem<Action>
{
    public static ActionItem Create(ItemPath parentPath, Action action)
    {
        var actionType = action.Type.Value;
        return actionType switch
        {
            "forward" => CreateForwardAction(parentPath, action),
            "redirect" => new RedirectActionItem(parentPath, action),
            "fixed-response" => new FixedActionItem(parentPath, action),
            _ => new DefaultActionItem(parentPath, action)
        };
    }

    private static ActionItem CreateForwardAction(ItemPath parentPath, Action action)
    {
        return action.ForwardConfig?.TargetGroups?.Count switch
        {
            > 1 => new WeightedForwardActionItem(parentPath, action),
            _ => new ForwardActionItem(parentPath, action)
        };
    }

    protected override string TypeName => typeof(ActionItem).FullName!;

    protected ActionItem(ItemPath parentPath, Action action) : base(parentPath, action)
    {
        ItemName = action.Type.Value;
    }
    
    public override string ItemName { get; }
    
    public abstract string Description { get; }

    protected override void CustomizePSObject(PSObject psObject)
    {
        psObject.Properties.Add(new PSNoteProperty("Description", Description));
    }

    public virtual IEnumerable<IItem> GetChildren(IAmazonElasticLoadBalancingV2 elbv2, LinkGenerator linkGenerator)
    {
        return Enumerable.Empty<IItem>();
    }
}