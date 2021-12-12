using Action = Amazon.ElasticLoadBalancingV2.Model.Action;

namespace MountAws.Services.ELBV2;

public class DefaultActionItem : ActionItem
{
    public DefaultActionItem(string parentPath, Action action) : base(parentPath, action)
    {
    }

    public override string ItemName => Action.Type;
    public override string ItemType => "Action";
    public override bool IsContainer => false;
    
    public override string Description => Action.Type;
}