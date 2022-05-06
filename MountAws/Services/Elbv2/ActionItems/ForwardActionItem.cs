using Amazon.ElasticLoadBalancingV2;
using MountAnything;
using Action = Amazon.ElasticLoadBalancingV2.Model.Action;

namespace MountAws.Services.Elbv2;

public class ForwardActionItem : ActionItem
{
    public ForwardActionItem(ItemPath parentPath, Action action) : base(parentPath, action)
    {
        TargetGroupArn = action.TargetGroupArn()!;
        TargetGroupName = action.TargetGroupName()!;
    }
    public override string ItemType => Elbv2ItemTypes.ForwardAction;
    public override bool IsContainer => true;

    public override string Description => $"Forwards to {TargetGroupName}";
    public string TargetGroupArn { get; }
    public string TargetGroupName { get; }

    public override IEnumerable<IItem> GetChildren(IAmazonElasticLoadBalancingV2 elbv2, LinkGenerator linkGenerator)
    {
        return new[]
        {
            new TargetGroupItem(FullPath, elbv2.GetTargetGroup(TargetGroupArn), linkGenerator)
        };
    }
}