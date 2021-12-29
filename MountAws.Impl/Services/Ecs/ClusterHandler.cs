using Amazon.ECS;
using Amazon.ECS.Model;
using MountAnything;
using MountAws.Api.AwsSdk.Ecs;

namespace MountAws.Services.Ecs;

public class ClusterHandler : PathHandler
{
    private readonly IAmazonECS _ecs;

    public ClusterHandler(string path, IPathHandlerContext context, IAmazonECS ecs) : base(path, context)
    {
        _ecs = ecs;
    }

    protected override IItem? GetItemImpl()
    {
        try
        {
            var cluster = _ecs.DescribeCluster(ItemName);

            return new ClusterItem(ParentPath, cluster);
        }
        catch (ClusterNotFoundException)
        {
            return null;
        }
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return ServicesHandler.CreateItem(Path);
        yield return ContainerInstancesHandler.CreateItem(Path);
    }
}