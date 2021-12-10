using Amazon.EC2;
using Amazon.EC2.Model;

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

    protected override AwsItem? GetItemImpl()
    {
        var request = new DescribeInstancesRequest();
        WriteDebug($"EC2InstanceHandler.GetItem({ItemName})");
        if (ItemName.StartsWith("i-"))
        {
            request.InstanceIds.Add(ItemName);
        }
        else
        {
            var filter = EC2ClientExtensions.ParseFilter(ItemName);
            WriteDebug($"GetItem({filter.Name}, {filter.Values.First()}");
            request.Filters.Add(filter);
        }
        var response = _ec2.DescribeInstancesAsync(request).GetAwaiter().GetResult();
        var instances = response.Reservations.SelectMany(r => r.Instances).ToArray();
        WriteDebug($"Found {instances.Length} instances");
        if (instances.Length == 1)
        {
            return new EC2InstanceItem(ParentPath, instances.First(), ItemName);
        }

        return null;
    }

    protected override IEnumerable<AwsItem> GetChildItemsImpl()
    {
        return Enumerable.Empty<AwsItem>();
    }
}