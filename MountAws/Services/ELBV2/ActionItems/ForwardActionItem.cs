using Amazon.ElasticLoadBalancingV2;
using Action = Amazon.ElasticLoadBalancingV2.Model.Action;

namespace MountAws.Services.ELBV2;

public class ForwardActionItem : ActionItem
{
    public ForwardActionItem(string parentPath, Action action) : base(parentPath, action)
    {
        TargetGroupArn = string.IsNullOrEmpty(Action.TargetGroupArn)
            ? Action.ForwardConfig.TargetGroups.Single().TargetGroupArn
            : Action.TargetGroupArn;
        TargetGroupName = ELBV2Extensions.TargetGroupName(TargetGroupArn);
    }

    public override string ItemName => "forward";
    public override string ItemType => "ForwardAction";
    public override bool IsContainer => true;

    public override string Description => $"Forwards to {TargetGroupName}";
    public string TargetGroupArn { get; }
    public string TargetGroupName { get; }

    public override IEnumerable<AwsItem> GetChildren(IAmazonElasticLoadBalancingV2 elbv2)
    {
        return new[]
        {
            new TargetGroupItem(FullPath, elbv2.GetTargetGroup(TargetGroupArn))
        };
    }
}