using Amazon.ECS;
using Amazon.ECS.Model;
using MountAnything;

namespace MountAws.Services.ECS;

public class ServiceHandler : PathHandler
{
    private readonly IAmazonECS _ecs;
    private readonly CurrentCluster _currentCluster;

    public ServiceHandler(string path, IPathHandlerContext context, IAmazonECS ecs, CurrentCluster currentCluster) : base(path, context)
    {
        _ecs = ecs;
        _currentCluster = currentCluster;
    }

    protected override Item? GetItemImpl()
    {
        var service = _ecs.DescribeServicesAsync(new DescribeServicesRequest
        {
            Cluster = _currentCluster.Name,
            Services = new List<string> { ItemName }
        }).GetAwaiter().GetResult().Services.FirstOrDefault();

        if (service != null)
        {
            return new ServiceItem(ParentPath, service);
        }

        return null;

    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        yield return ServiceTasksHandler.CreateItem(Path);
    }
}