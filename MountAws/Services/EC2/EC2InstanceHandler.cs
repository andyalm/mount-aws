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
        var response = _ec2.DescribeInstancesAsync(new DescribeInstancesRequest
        {
            InstanceIds = new List<string>
            {
                ItemName
            }
        }).GetAwaiter().GetResult();

        return new EC2InstanceItem(ParentPath, response.Reservations[0].Instances[0]);
    }

    protected override IEnumerable<AwsItem> GetChildItemsImpl()
    {
        return Enumerable.Empty<AwsItem>();
    }
}