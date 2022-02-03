using Amazon.AutoScaling;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Ec2;

public class AutoScalingGroupsHandler : PathHandler
{
    private readonly IAmazonAutoScaling _autoScaling;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "auto-scaling-groups",
            "Navigate autoscaling groups and their instances");
    }

    public AutoScalingGroupsHandler(ItemPath path, IPathHandlerContext context, IAmazonAutoScaling autoScaling) : base(path, context)
    {
        _autoScaling = autoScaling;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _autoScaling.DescribeAutoScalingGroups().Select(g => new AutoScalingGroupItem(Path, g, LinkGenerator));
    }

    public override IEnumerable<IItem> GetChildItems(string filter)
    {
        // asg api doesn't support wildcard name filters, so we fallback to base implementation that does that client side
        if (!filter.Contains("=") && filter.Contains("*"))
        {
            return base.GetChildItems(filter);
        }
        
        return _autoScaling.DescribeAutoScalingGroups(filter).Select(g => new AutoScalingGroupItem(Path, g, LinkGenerator));
    }
}