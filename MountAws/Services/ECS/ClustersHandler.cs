using Amazon.ECS;
using Amazon.ECS.Model;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.ECS;

public class ClustersHandler : PathHandler
{
    private readonly IAmazonECS _ecs;

    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "clusters",
            "Navigate the ECS clusters in this account and region");
    }
    
    public ClustersHandler(string path, IPathHandlerContext context, IAmazonECS ecs) : base(path, context)
    {
        _ecs = ecs;
    }

    protected override Item? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        var clusterArns = _ecs.ListClustersAsync(new ListClustersRequest()).GetAwaiter().GetResult().ClusterArns;
        var clusters = _ecs.DescribeClustersAsync(new DescribeClustersRequest
        {
            Clusters = new List<string>(clusterArns)
        }).GetAwaiter().GetResult().Clusters;

        return clusters.Select(c => new ClusterItem(Path, c)).OrderBy(c => c.ItemName);
    }
}