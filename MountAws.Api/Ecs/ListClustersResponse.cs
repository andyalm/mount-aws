namespace MountAws.Api.Ecs;

public class ListClustersResponse
{
    public ListClustersResponse(IEnumerable<string> clusterArns, string? nextToken)
    {
        ClusterArns = clusterArns.ToArray();
        NextToken = nextToken;
    }

    public string[] ClusterArns { get; }
    public string? NextToken { get; }
}