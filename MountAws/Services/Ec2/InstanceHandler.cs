using MountAnything;
using MountAws.Api.Ec2;

namespace MountAws.Services.Ec2;

public class InstanceHandler : PathHandler, IRemoveItemHandler
{
    private readonly IEc2Api _ec2;

    public InstanceHandler(string path, IPathHandlerContext context, IEc2Api ec2) : base(path, context)
    {
        _ec2 = ec2;
    }

    protected override IItem? GetItemImpl()
    {
        // don't allow an arbitrary filter value. Only names or ip's
        if (ItemName.Contains("="))
        {
            return null;
        }
        
        var request = Ec2ApiExtensions.ParseInstanceFilter(ItemName);
        var instances = _ec2.DescribeInstances(request).Instances.ToArray();
        WriteDebug($"Found {instances.Length} instances");
        if (instances.Length == 1)
        {
            return new InstanceItem(ParentPath, instances.Single());
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