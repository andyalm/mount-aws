namespace MountAws.Api.Ec2;

public class DescribeSecurityGroupsRequest
{
    public List<string> Ids { get; set; } = new();
    public List<Filter> Filters { get; set; } = new();
    public string? NextToken { get; set; }
}