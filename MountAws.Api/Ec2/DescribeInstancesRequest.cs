namespace MountAws.Api.Ec2;

public class DescribeInstancesRequest
{
    public List<string> InstanceIds { get; set; } = new();
    public List<Filter> Filters { get; set; } = new();
    public string? NextToken { get; set; }
}