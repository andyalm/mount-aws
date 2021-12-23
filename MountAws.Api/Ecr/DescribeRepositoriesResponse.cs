using System.Management.Automation;

namespace MountAws.Api.Ecr;

public class DescribeRepositoriesResponse
{
    public DescribeRepositoriesResponse(IEnumerable<PSObject> repositories, string? nextToken)
    {
        Repositories = repositories.ToArray();
        NextToken = nextToken;
    }

    public PSObject[] Repositories { get; }
    public string? NextToken { get; }
}