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
    
    private readonly IEc2Api _ec2;

    public InstancesHandler(string path, IPathHandlerContext context, IEc2Api ec2) : base(path, context)
    {
        _ec2 = ec2;
    }

    protected override bool ExistsImpl()
    {
        return true;
    }

    protected override Item? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        return _ec2.QueryInstances()
            .Select(i => new InstanceItem(Path, i));
    }

    public override IEnumerable<Item> GetChildItems(string filter)
    {
        return _ec2.QueryInstances(filter)
            .Select(instance => new InstanceItem(Path, instance));
    }

    public override bool CacheChildren => false;
}