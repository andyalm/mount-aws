using System.Management.Automation;
using MountAnything;
using MountAws.Api;
using MountAws.Api.Elbv2;

namespace MountAws.Services.Elbv2;

public class WeightedForwardActionItem : ActionItem
{
    public WeightedForwardActionItem(string parentPath, PSObject action) : base(parentPath, action)
    {
        WeightedTargetGroups = action.Property<PSObject>("ForwardConfig")!
            .Property<IEnumerable<PSObject>>("TargetGroups")!
            .ToArray();
        WeightDescriptions = WeightedTargetGroups
            .Select(t => $"{t.Property<string>("Weight")}:${Elbv2ApiExtensions.TargetGroupName(t.Property<string>("TargetGroupArn")!)}")
            .ToArray();
    }

    public override string ItemName => "forward";
    public override string ItemType => "ForwardAction";
    public override bool IsContainer => true;

    public override string Description => $"Forward with weights {string.Join(",", WeightDescriptions)}";
    
    public PSObject[] WeightedTargetGroups { get; }
    public string[] WeightDescriptions { get; }

    public override IEnumerable<Item> GetChildren(IElbv2Api elbv2)
    {
        return WeightedTargetGroups.Select(t =>
            new WeightedTargetGroupItem(FullPath, elbv2.GetTargetGroup(t.Property<string>("TargetGroupArn")!), t.Property<int>("Weight")));
    }
}