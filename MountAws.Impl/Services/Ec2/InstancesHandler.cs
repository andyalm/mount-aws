using Amazon.EC2;
using MountAnything;
using MountAws.Api.Ec2;
using MountAws.Services.Core;

namespace MountAws.Services.Ec2;

public class InstancesHandler : PathHandler
{
    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "instances",
            "Find all the ec2 instances within the current account and region");
    }
    
    private readonly IAmazonEC2 _ec2;

    public InstancesHandler(string path, IPathHandlerContext context, IAmazonEC2 ec2) : base(path, context)
    {
        _ec2 = ec2;
    }

    protected override bool ExistsImpl()
    {
        return true;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _ec2.QueryInstances()
            .Select(i => new InstanceItem(Path, i));
    }

    public override IEnumerable<IItem> GetChildItems(string filter)
    {
        return _ec2.QueryInstances(filter)
            .Select(instance => new InstanceItem(Path, instance));
    }

    protected override bool CacheChildren => false;
}