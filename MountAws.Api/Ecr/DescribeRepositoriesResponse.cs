using System.Management.Automation;

namespace MountAws.Api.Ecr;

public class DescribeRepositoriesResponse
{
    public PSObject[] Repositories { get; init; }
    public string NextToken { get; init; }
}