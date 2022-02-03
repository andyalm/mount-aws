using Amazon.EC2;
using MountAnything;
using MountAws.Api.Ec2;

namespace MountAws.Services.Ec2;

public class InstanceHandler : PathHandler, IRemoveItemHandler
{
    private readonly IAmazonEC2 _ec2;

    public InstanceHandler(ItemPath path, IPathHandlerContext context, IAmazonEC2 ec2) : base(path, context)
    {
        _ec2 = ec2;
    }

    protected override IItem? GetItemImpl()
    {
        var request = Ec2ApiExtensions.ParseInstanceFilter(ItemName);
        if (request.Filters.Any(f => f.Name == "tag:Name"))
        {
            Context.WriteWarning("If you want to get an item by name, use a wildcard");
            return null;
        }
        
        var instances = _ec2.DescribeInstances(request).ToArray();
        WriteDebug($"Found {instances.Length} instances");
        if (instances.Length == 1)
        {
            return new InstanceItem(ParentPath, instances.Single(), LinkGenerator);
        }

        return null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return Enumerable.Empty<Item>();
    }

    public void RemoveItem()
    {
        var item = GetItem(Freshness.Fastest);
        if (item == null)
        {
            throw new InvalidOperationException($"Instance '{ItemName}' does not exist");
        }

        _ec2.TerminateInstance(item.ItemName);
    }
}