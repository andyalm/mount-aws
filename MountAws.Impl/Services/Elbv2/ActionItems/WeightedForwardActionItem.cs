using Amazon.ElasticLoadBalancingV2;
using Amazon.ElasticLoadBalancingV2.Model;
using MountAnything;
using Action = Amazon.ElasticLoadBalancingV2.Model.Action;

namespace MountAws.Services.Elbv2;

public class WeightedForwardActionItem : ActionItem
{
    public WeightedForwardActionItem(ItemPath parentPath, Action action) : base(parentPath, action)
    {
        WeightedTargetGroups = action.ForwardConfig
            .TargetGroups
            .ToArray();
        WeightDescriptions = WeightedTargetGroups
            .Select(t => $"{t.Weight}:${t.TargetGroupName()}")
            .ToArray();
    }
    public override string ItemType => Elbv2ItemTypes.ForwardAction;
    public override bool IsContainer => true;

    public override string Description => $"Forward with weights {string.Join(",", WeightDescriptions)}";
    
    public TargetGroupTuple[] WeightedTargetGroups { get; }
    public string[] WeightDescriptions { get; }

    public override IEnumerable<IItem> GetChildren(IAmazonElasticLoadBalancingV2 elbv2)
    {
        return WeightedTargetGroups.Select(t =>
            new WeightedTargetGroupItem(FullPath, elbv2.GetTargetGroup(t.TargetGroupArn), t.Weight));
    }
}