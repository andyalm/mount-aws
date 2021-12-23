namespace MountAws.Api.Elbv2;

public class DescribeTargetGroupsRequest
{
    public List<string> Names { get; set; } = new();
    public List<string> Arns { get; set; } = new();
}