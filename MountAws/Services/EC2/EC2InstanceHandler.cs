using Amazon.EC2;
using Amazon.EC2.Model;
using MountAnything;

namespace MountAws.Services.EC2;

public class EC2InstanceHandler : PathHandler
{
    private readonly IAmazonEC2 _ec2;

    public EC2InstanceHandler(string path, IPathHandlerContext context, IAmazonEC2 ec2) : base(path, context)
    {
        _ec2 = ec2;
    }

    protected override bool ExistsImpl()
    {
        return GetItem() != null;
    }

    protected override Item? GetItemImpl()
    {
        var request = EC2ClientExtensions.ParseFilter(ItemName);
        var response = _ec2.DescribeInstancesAsync(request).GetAwaiter().GetResult();
        var instances = response.Reservations.SelectMany(r => r.Instances).ToArray();
        WriteDebug($"Found {instances.Length} instances");
        if (instances.Length == 1)
        {
            return new EC2InstanceItem(ParentPath, instances.First(), ItemName);
        }

        return null;
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        return Enumerable.Empty<Item>();
    }
}