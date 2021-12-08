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

public class EC2InstanceItem : AwsItem
{
    private readonly Instance _ec2Instance;
    
    public EC2InstanceItem(string parentPath, Instance instance)
    {
        ParentPath = parentPath;
        _ec2Instance = instance;
    }

    public string ParentPath { get; }
    public override string FullPath => AwsPath.Combine(ParentPath, Name);
    public override string Name => _ec2Instance.InstanceId;
    public override object UnderlyingObject => _ec2Instance;
    public override string ItemType => "EC2Instance";
    public override bool IsContainer => false;
}