using System.Management.Automation;

namespace MountAws.Api.Ecs;

public class ListServicesResponse
{
    public ListServicesResponse(IEnumerable<string> serviceArns, string? nextToken)
    {
        ServiceArns = serviceArns.ToArray();
        NextToken = nextToken;
    }

    public string[] ServiceArns { get; }
    public string? NextToken { get; }
}