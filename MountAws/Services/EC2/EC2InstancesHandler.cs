using System.Text.RegularExpressions;
using Amazon.EC2;
using Amazon.EC2.Model;

namespace MountAws.Services.EC2;

public class EC2InstancesHandler : PathHandler, IGetChildItemParameters<EC2QueryParameters>
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
        return _ec2.QueryInstances(Context.Filter, GetChildItemParameters)
            .Select(i => new EC2InstanceItem(Path, i));
    }

    public override IEnumerable<string> ExpandPath(string pattern)
    {
        if (pattern.StartsWith("i-"))
        {
            return base.ExpandPath(pattern);
        }
        
        return _ec2.QueryInstances(pattern)
            .Select(instance => GetItemNameForPattern(instance, pattern))
            .Select(itemName => AwsPath.Combine(Path, itemName));
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
}