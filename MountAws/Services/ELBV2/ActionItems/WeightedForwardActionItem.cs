using Amazon.ElasticLoadBalancingV2;
using Amazon.ElasticLoadBalancingV2.Model;
using Action = Amazon.ElasticLoadBalancingV2.Model.Action;

namespace MountAws.Services.ELBV2;

public class WeightedForwardActionItem : ActionItem
{
    public WeightedForwardActionItem(string parentPath, Action action) : base(parentPath, action)
    {
        WeightedTargetGroups = action.ForwardConfig.TargetGroups.ToArray();
        WeightDescriptions = action.ForwardConfig.TargetGroups
            .Select(t => $"{t.Weight}:${ELBV2Extensions.TargetGroupName(t.TargetGroupArn)}")
            .ToArray();
    }

    public override string ItemName => "forward";
    public override string ItemType => "ForwardAction";
    public override bool IsContainer => true;

    public override string Description => $"Forward with weights {string.Join(",", WeightDescriptions)}";
    
    public TargetGroupTuple[] WeightedTargetGroups { get; }
    public string[] WeightDescriptions { get; }

    public override IEnumerable<AwsItem> GetChildren(IAmazonElasticLoadBalancingV2 elbv2)
    {
        return WeightedTargetGroups.Select(t =>
            new WeightedTargetGroupItem(FullPath, elbv2.GetTargetGroup(t.TargetGroupArn), t.Weight));
    }
}