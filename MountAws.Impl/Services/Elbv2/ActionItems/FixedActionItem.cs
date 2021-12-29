using Action = Amazon.ElasticLoadBalancingV2.Model.Action;

namespace MountAws.Services.Elbv2;

public class FixedActionItem : ActionItem
{
    public FixedActionItem(string parentPath, Action action) : base(parentPath, action) { }

    public override string ItemType => Elbv2ItemTypes.FixedAction;
    public override bool IsContainer => false;
    public override string Description => $"Fixed {UnderlyingObject.FixedResponseConfig.StatusCode} response";
}