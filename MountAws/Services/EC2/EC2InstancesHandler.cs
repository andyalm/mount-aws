using System.Text.RegularExpressions;
using Amazon.EC2;
using Amazon.EC2.Model;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.EC2;

public class EC2InstancesHandler : PathHandler, IGetChildItemParameters<EC2QueryParameters>
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
        return _ec2.QueryInstances(Context.Filter, GetChildItemParameters)
            .Select(i => new EC2InstanceItem(Path, i));
    }

    public override IEnumerable<string> ExpandPath(string pattern)
    {
        return _ec2.QueryInstances(pattern)
            .Select(instance => (Instance: instance,ItemName:GetItemNameForPattern(instance, pattern)))
            .ForEach(instance => CacheInstance(instance.Instance, instance.ItemName))
            .Select(instance => ItemPath.Combine(Path, instance.ItemName));
    }

    private void CacheInstance(Instance instance, string itemName)
    {
        var itemNames = new[] { instance.InstanceId, itemName }.Distinct();
        foreach (var anItemName in itemNames)
        {
            Cache.SetItem(new EC2InstanceItem(ParentPath, instance, anItemName));
        }
    }

    private string GetItemNameForPattern(Instance instance, string pattern)
    {
        if (pattern.StartsWith("ip-") || Regex.IsMatch(pattern, @"^[0-9\.\*]+$"))
        {
            return instance.PrivateIpAddress;
        }

        return instance.InstanceId;
    }

    public EC2QueryParameters GetChildItemParameters { get; set; } = new();
    public override bool CacheChildren => false;
}