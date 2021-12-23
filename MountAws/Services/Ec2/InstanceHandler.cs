using MountAnything;
using MountAws.Api.Ec2;

namespace MountAws.Services.Ec2;

public class InstanceHandler : PathHandler
{
    private readonly IEc2Api _ec2;

    public InstanceHandler(string path, IPathHandlerContext context, IEc2Api ec2) : base(path, context)
    {
        _ec2 = ec2;
    }

    protected override Item? GetItemImpl()
    {
        var request = Ec2ApiExtensions.ParseFilter(ItemName);
        var instances = _ec2.DescribeInstances(request).ToArray();
        WriteDebug($"Found {instances.Length} instances");
        if (instances.Length == 1)
        {
            return new InstanceItem(ParentPath, instances.First());
        }

        return null;
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        return Enumerable.Empty<Item>();
    }
}