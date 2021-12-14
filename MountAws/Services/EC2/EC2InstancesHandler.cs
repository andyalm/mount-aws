using System.Text.RegularExpressions;
using Amazon.EC2;
using Amazon.EC2.Model;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.EC2;

public class EC2InstancesHandler : PathHandler
{
    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "instances",
            "Find all the ec2 instances within the current account and region");
    }
    
    private readonly IAmazonEC2 _ec2;

    public static Item GetItem(string parentPath)
    {
        return CreateItem(parentPath);
    }
    
    public EC2InstancesHandler(string path, IPathHandlerContext context, IAmazonEC2 ec2) : base(path, context)
    {
        _ec2 = ec2;
    }

    protected override bool ExistsImpl()
    {
        return true;
    }

    protected override Item? GetItemImpl()
    {
        return GetItem(ParentPath);
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        return _ec2.QueryInstances()
            .Select(i => new EC2InstanceItem(Path, i));
    }

    public override IEnumerable<Item> GetChildItems(string filter)
    {
        return _ec2.QueryInstances(filter)
            .Select(instance => new EC2InstanceItem(Path, instance));
    }

    public override bool CacheChildren => false;
}