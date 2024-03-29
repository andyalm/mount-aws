using Amazon.AutoScaling;
using Amazon.EC2;
using Amazon.EC2.Model;
using MountAnything;

namespace MountAws.Services.Ec2;

public class AutoScalingGroupHandler : PathHandler
{
    private readonly IAmazonAutoScaling _autoScaling;
    private readonly IAmazonEC2 _ec2;

    public AutoScalingGroupHandler(ItemPath path, IPathHandlerContext context, IAmazonAutoScaling autoScaling, IAmazonEC2 ec2) : base(path, context)
    {
        _autoScaling = autoScaling;
        _ec2 = ec2;
    }

    protected override IItem? GetItemImpl()
    {
        var asg = _autoScaling.DescribeAutoScalingGroup(ItemName);

        return new AutoScalingGroupItem(ParentPath, asg, LinkGenerator);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var asg = GetItem() as AutoScalingGroupItem;
        if (asg == null)
        {
            return Enumerable.Empty<IItem>();
        }

        var instanceIds = asg.UnderlyingObject.Instances.Select(i => i.InstanceId).ToList();
        var instances = _ec2.DescribeInstances(new DescribeInstancesRequest
        {
            InstanceIds = instanceIds.ToList()
        }).ToList();
        var images = _ec2.DescribeImages(instances.Select(i => i.ImageId).Distinct()).ToDictionary(i => i.ImageId);

        return instances.Select(i => new InstanceItem(Path, i, images.GetValueOrDefault(i.ImageId), LinkGenerator));
    }
}