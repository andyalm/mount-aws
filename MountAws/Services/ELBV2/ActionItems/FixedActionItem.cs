using Action = Amazon.ElasticLoadBalancingV2.Model.Action;

namespace MountAws.Services.ELBV2;

public class FixedActionItem : ActionItem
{
    public FixedActionItem(string parentPath, Action action) : base(parentPath, action)
    {
    }

    public override string ItemName => Action.Type.ToString();
    public override string ItemType => "FixedAction";
    public override bool IsContainer => false;
    public override string Description => $"Fixed {Action.FixedResponseConfig.StatusCode} response";
}