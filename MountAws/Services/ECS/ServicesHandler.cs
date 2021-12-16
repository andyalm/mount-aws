using Amazon.ECS;
using Amazon.ECS.Model;
using MountAnything;
using MountAws.Services.Core;

using static MountAws.PagingHelper;

namespace MountAws.Services.ECS;

public class ServicesHandler : PathHandler
{
    private readonly IAmazonECS _ecs;
    private readonly CurrentCluster _currentCluster;

    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "services",
            "Navigate the ECS services and its tasks and other related objects");
    }
    
    public ServicesHandler(string path, IPathHandlerContext context, IAmazonECS ecs, CurrentCluster currentCluster) : base(path, context)
    {
        _ecs = ecs;
        _currentCluster = currentCluster;
    }

    protected override Item? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        var serviceArns = GetWithPaging(nextToken =>
        {
            var response = _ecs.ListServicesAsync(new ListServicesRequest
            {
                Cluster = _currentCluster.Name,
                MaxResults = 100
            }).GetAwaiter().GetResult();

            return new PaginatedResponse<string>
            {
                NextToken = response.NextToken,
                PageOfResults = response.ServiceArns.ToArray()
            };
        });

        return serviceArns.Chunk(10).SelectMany(serviceArnChunk =>
        {
            var response = _ecs.DescribeServicesAsync(new DescribeServicesRequest
            {
                Cluster = _currentCluster.Name,
                Services = new List<string>(serviceArnChunk),
                Include = new List<string>{"TAGS"}
            }).GetAwaiter().GetResult();

            return response.Services;
        }).Select(s => new ServiceItem(Path, s)).OrderBy(s => s.ItemName);
    }
}