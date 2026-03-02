using Amazon.ECS;
using MountAnything;
using MountAws.Api.AwsSdk.Ecs;
using MountAws.Services.Core;

using static MountAws.PagingHelper;

namespace MountAws.Services.Ecs;

public class ClustersHandler(ItemPath path, IPathHandlerContext context, IAmazonECS ecs) : PathHandler(path, context)
{
    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "clusters",
            "Navigate the ECS clusters in this account and region");
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var clusterArns = ecs.ListClusters();
        var clusters = ecs.DescribeClusters(clusterArns, new []{"TAGS"});

        return clusters.Select(c => new ClusterItem(Path, c)).OrderBy(c => c.ItemName);
    }
}