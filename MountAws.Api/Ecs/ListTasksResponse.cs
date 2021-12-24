namespace MountAws.Api.Ecs;

public class ListTasksResponse
{
    public ListTasksResponse(IEnumerable<string> taskArns, string? nextToken)
    {
        TaskArns = taskArns.ToArray();
        NextToken = nextToken;
    }

    public string[] TaskArns { get; }
    public string? NextToken { get; }
}