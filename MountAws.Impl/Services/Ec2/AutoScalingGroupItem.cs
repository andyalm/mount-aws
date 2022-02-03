using Amazon.AutoScaling.Model;
using MountAnything;
using MountAws.Services.Elbv2;

namespace MountAws.Services.Ec2;

public class AutoScalingGroupItem : AwsItem<AutoScalingGroup>
{
    public AutoScalingGroupItem(ItemPath parentPath, AutoScalingGroup underlyingObject, LinkGenerator linkGenerator) : base(parentPath, underlyingObject)
    {
        ItemName = underlyingObject.AutoScalingGroupName;
        var targetGroupArn = underlyingObject.TargetGroupARNs.FirstOrDefault();
        if (targetGroupArn != null)
        {
            LinkPaths = new Dictionary<string, ItemPath>
            {
                ["TargetGroup"] = linkGenerator.TargetGroup(targetGroupArn)
            };
        }
    }

    public override string ItemName { get; }
    public override bool IsContainer => true;

    [ItemProperty]
    public int? NumInstances => UnderlyingObject.Instances?.Count;

    [ItemProperty]
    public IEnumerable<string> InstanceTypes => UnderlyingObject.Instances.Select(i => i.InstanceType).Distinct();

    public override string? WebUrl => UrlBuilder.CombineWith($"ec2autoscaling/home#/details/{ItemName}");
}