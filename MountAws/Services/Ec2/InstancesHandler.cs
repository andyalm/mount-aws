using Amazon.EC2;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Ec2;

public class InstancesHandler : PathHandler
{
    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "instances",
            "Find all the ec2 instances within the current account and region");
    }
    
    private readonly IAmazonEC2 _ec2;

    public InstancesHandler(ItemPath path, IPathHandlerContext context, IAmazonEC2 ec2) : base(path, context)
    {
        _ec2 = ec2;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var instances = _ec2.QueryInstances().ToArray();
        var images = _ec2.DescribeImages(instances.Select(i => i.ImageId).Distinct()).ToDictionary(i => i.ImageId);    
        
        return instances.Select(i => new InstanceItem(Path, i, images.GetValueOrDefault(i.ImageId), LinkGenerator));
    }

    public override IEnumerable<IItem> GetChildItems(string filter)
    {
        var instances = _ec2.QueryInstances(filter).ToArray();
        var images = _ec2.DescribeImages(instances.Select(i => i.ImageId).Distinct()).ToDictionary(i => i.ImageId);    
    
        return instances.Select(instance => new InstanceItem(Path, instance, images.GetValueOrDefault(instance.ImageId), LinkGenerator));
    }

    protected override bool CacheChildren => false;
}