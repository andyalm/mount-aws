using System.Management.Automation;
using Amazon.ElasticLoadBalancingV2;
using MountAnything;
using Action = Amazon.ElasticLoadBalancingV2.Model.Action;

namespace MountAws.Services.ELBV2;

public abstract class ActionItem : Item
{
    public static ActionItem Create(string parentPath, Action action)
    {
        if (action.Type == ActionTypeEnum.Forward && action.ForwardConfig?.TargetGroups?.Count > 1)
        {
            return new WeightedForwardActionItem(parentPath, action);
        }
        if (action.Type == ActionTypeEnum.Forward)
        {
            return new ForwardActionItem(parentPath, action);
        }
        if (action.Type == ActionTypeEnum.Redirect)
        {
            return new RedirectActionItem(parentPath, action);
        }
        if (action.Type == ActionTypeEnum.FixedResponse)
        {
            return new FixedActionItem(parentPath, action);
        }

        return new DefaultActionItem(parentPath, action);
    }
    
    public Action Action { get; }

    public override string TypeName => "MountAws.Services.ELBV2.ActionItem";

    protected ActionItem(string parentPath, Action action) : base(parentPath)
    {
        Action = action;
    }
    public abstract string Description { get; }
    public override object UnderlyingObject => Action;

    public override void CustomizePSObject(PSObject psObject)
    {
        psObject.Properties.Add(new PSNoteProperty("Description", Description));
    }

    public virtual IEnumerable<Item> GetChildren(IAmazonElasticLoadBalancingV2 elbv2)
    {
        return Enumerable.Empty<Item>();
    }
}