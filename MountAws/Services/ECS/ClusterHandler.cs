using Amazon.ECS;
using Amazon.ECS.Model;
using MountAnything;

namespace MountAws.Services.ECS;

public class ClusterHandler : PathHandler
{
    private readonly IAmazonECS _ecs;

    public ClusterHandler(string path, IPathHandlerContext context, IAmazonECS ecs) : base(path, context)
    {
        _ecs = ecs;
    }

    protected override Item? GetItemImpl()
    {
        var cluster = _ecs.DescribeClustersAsync(new DescribeClustersRequest
        {
            Clusters = new List<string> { ItemName }
        }).GetAwaiter().GetResult().Clusters.FirstOrDefault();

        if (cluster != null)
        {
            return new ClusterItem(ParentPath, cluster);
        }

        return null;
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        yield return ServicesHandler.CreateItem(Path);
    }
}