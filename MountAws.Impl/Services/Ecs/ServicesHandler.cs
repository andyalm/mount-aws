using Amazon.ECS;
using MountAnything;
using MountAws.Api.AwsSdk.Ecs;
using MountAws.Services.Core;

namespace MountAws.Services.Ecs;

public class ServicesHandler : PathHandler
{
    private readonly IAmazonECS _ecs;
    private readonly CurrentCluster _currentCluster;

    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "services",
            "Navigate the ECS services and its tasks and other related objects");
    }
    
    public ServicesHandler(ItemPath path, IPathHandlerContext context, IAmazonECS ecs, CurrentCluster currentCluster) : base(path, context)
    {
        _ecs = ecs;
        _currentCluster = currentCluster;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var serviceArns = _ecs.ListServices(_currentCluster.Name);

        return serviceArns.Chunk(10).SelectMany(serviceArnChunk =>
        {
            return _ecs.DescribeServices(_currentCluster.Name,
                serviceArnChunk,
                new[] { "TAGS" });
        }).Select(s => new ServiceItem(Path, s, LinkGenerator))
            .OrderBy(s => s.ItemName);
    }
}