using Amazon.ECS;
using MountAnything;
using MountAws.Api.AwsSdk.Ecs;
using MountAws.Services.Core;

using static MountAws.PagingHelper;

namespace MountAws.Services.Ecs;

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

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var clusterArns = _ecs.ListClusters();
        var clusters = _ecs.DescribeClusters(clusterArns, new []{"TAGS"});

        return clusters.Select(c => new ClusterItem(Path, c)).OrderBy(c => c.ItemName);
    }
}