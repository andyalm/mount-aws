using System.Management.Automation;
using Amazon.ElasticLoadBalancingV2;
using MountAnything;
using Action = Amazon.ElasticLoadBalancingV2.Model.Action;

namespace MountAws.Services.Elbv2;

public abstract class ActionItem : AwsItem<Action>
{
    public static ActionItem Create(string parentPath, Action action)
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

    private static ActionItem CreateForwardAction(string parentPath, Action action)
    {
        if (action.ForwardConfig?.TargetGroups?.Count > 1)
        {
            return new WeightedForwardActionItem(parentPath, action);
        }
        else
        {
            return new ForwardActionItem(parentPath, action);
        }
    }
    
    public override string TypeName => typeof(ActionItem).FullName!;

    protected ActionItem(string parentPath, Action action) : base(parentPath, action)
    {
        ItemName = action.Type.Value;
    }
    
    public override string ItemName { get; }
    
    public abstract string Description { get; }
    public override void CustomizePSObject(PSObject psObject)
    {
        psObject.Properties.Add(new PSNoteProperty("Description", Description));
    }

    public virtual IEnumerable<IItem> GetChildren(IAmazonElasticLoadBalancingV2 elbv2)
    {
        return Enumerable.Empty<IItem>();
    }
}