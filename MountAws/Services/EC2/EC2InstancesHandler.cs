using Amazon.EC2;
using Amazon.EC2.Model;

namespace MountAws.Services.EC2;

public class EC2InstancesHandler : PathHandler
{
    private readonly IAmazonEC2 _ec2;

    public static AwsItem GetItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "instances");
    }
    
    public EC2InstancesHandler(string path, IPathHandlerContext context, IAmazonEC2 ec2) : base(path, context)
    {
        _ec2 = ec2;
    }

    protected override bool ExistsImpl()
    {
        return true;
    }

    protected override AwsItem? GetItemImpl()
    {
        return GetItem(ParentPath);
    }

    protected override IEnumerable<AwsItem> GetChildItemsImpl()
    {
        var response = _ec2.DescribeInstancesAsync(new DescribeInstancesRequest
        {
            MaxResults = 100
        }).GetAwaiter().GetResult();

        return response.Reservations.SelectMany(r => r.Instances.Select(i => new EC2InstanceItem(Path, i)));
    }
}