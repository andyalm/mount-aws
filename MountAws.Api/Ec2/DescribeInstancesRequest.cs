namespace MountAws.Api.Ec2;

public class DescribeInstancesRequest
{
    public List<string> InstanceIds { get; set; } = new();
    public List<KeyValuePair<string, string>> Filters { get; } = new();
}