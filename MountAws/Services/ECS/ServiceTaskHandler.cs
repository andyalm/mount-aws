using Amazon.ECS;
using Amazon.ECS.Model;
using MountAnything;

namespace MountAws.Services.ECS;

public class ServiceTaskHandler : PathHandler
{
    private readonly IAmazonECS _ecs;
    private readonly CurrentCluster _currentCluster;

    public ServiceTaskHandler(string path, IPathHandlerContext context, IAmazonECS ecs, CurrentCluster currentCluster) : base(path, context)
    {
        _ecs = ecs;
        _currentCluster = currentCluster;
    }

    protected override Item? GetItemImpl()
    {
        var task = _ecs.DescribeTasksAsync(new DescribeTasksRequest
        {
            Cluster = _currentCluster.Name,
            Include = new List<string> { "TAGS" },
            Tasks = new List<string> { ItemName }
        }).GetAwaiter().GetResult().Tasks.SingleOrDefault();

        if (task != null)
        {
            return new TaskItem(ParentPath, task);
        }

        return null;
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        yield break;
    }
}