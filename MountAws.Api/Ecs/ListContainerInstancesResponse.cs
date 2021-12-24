using System.Management.Automation;

namespace MountAws.Api.Ecs;

public class ListContainerInstancesResponse
{
    public ListContainerInstancesResponse(IEnumerable<string> containerInstanceArns, string? nextToken)
    {
        ContainerInstanceArns = containerInstanceArns.ToArray();
        NextToken = nextToken;
    }

    public string[] ContainerInstanceArns { get; }
    public string? NextToken { get; }
}