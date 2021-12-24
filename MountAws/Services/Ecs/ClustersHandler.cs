using MountAnything;
using MountAws.Api.Ecs;
using MountAws.Services.Core;

using static MountAws.PagingHelper;

namespace MountAws.Services.Ecs;

public class ClustersHandler : PathHandler
{
    private readonly IEcsApi _ecs;

    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "clusters",
            "Navigate the ECS clusters in this account and region");
    }
    
    public ClustersHandler(string path, IPathHandlerContext context, IEcsApi ecs) : base(path, context)
    {
        _ecs = ecs;
    }

    protected override Item? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        var clusterArns = GetWithPaging(nextToken =>
        {
            var response = _ecs.ListClusters(nextToken);

            return new PaginatedResponse<string>
            {
                PageOfResults = response.ClusterArns,
                NextToken = response.NextToken
            };
        });
        var clusters = _ecs.DescribeClusters(clusterArns, new []{"TAGS"});

        return clusters.Select(c => new ClusterItem(Path, c)).OrderBy(c => c.ItemName);
    }
}