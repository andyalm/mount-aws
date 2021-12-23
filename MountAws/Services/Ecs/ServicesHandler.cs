using MountAnything;
using MountAws.Api.Ecs;
using MountAws.Services.Core;

using static MountAws.PagingHelper;

namespace MountAws.Services.Ecs;

public class ServicesHandler : PathHandler
{
    private readonly IEcsApi _ecs;
    private readonly CurrentCluster _currentCluster;

    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "services",
            "Navigate the ECS services and its tasks and other related objects");
    }
    
    public ServicesHandler(string path, IPathHandlerContext context, IEcsApi ecs, CurrentCluster currentCluster) : base(path, context)
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
            var response = _ecs.ListServices(_currentCluster.Name);

            return new PaginatedResponse<string>
            {
                PageOfResults = response.ServiceArns,
                NextToken = response.NextToken,
            };
        });

        return serviceArns.Chunk(10).SelectMany(serviceArnChunk =>
        {
            return _ecs.DescribeServices(_currentCluster.Name,
                serviceArnChunk,
                new[] { "TAGS" });
        }).Select(s => new ServiceItem(Path, s))
            .OrderBy(s => s.ItemName);
    }
}