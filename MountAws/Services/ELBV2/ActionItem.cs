using System.Management.Automation;
using Amazon.ElasticLoadBalancingV2;
using Action = Amazon.ElasticLoadBalancingV2.Model.Action;

namespace MountAws.Services.ELBV2;

public abstract class ActionItem : AwsItem
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
    
    public string ParentPath { get; }
    public Action Action { get; }

    public override string TypeName => "MountAws.Services.ELBV2.ActionItem";

    protected ActionItem(string parentPath, Action action)
    {
        ParentPath = parentPath;
        Action = action;
    }
    public abstract string Description { get; }
    public override string FullPath => AwsPath.Combine(ParentPath, ItemName);
    public override object UnderlyingObject => Action;

    public override void CustomizePSObject(PSObject psObject)
    {
        psObject.Properties.Add(new PSNoteProperty("Description", Description));
    }

    public virtual IEnumerable<AwsItem> GetChildren(IAmazonElasticLoadBalancingV2 elbv2)
    {
        return Enumerable.Empty<AwsItem>();
    }
}