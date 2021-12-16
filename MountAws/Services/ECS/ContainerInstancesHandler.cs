using System.Text.RegularExpressions;
using Amazon.ECS;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.ECS;

public class ContainerInstancesHandler : PathHandler
{
    private readonly IAmazonECS _ecs;
    private readonly CurrentCluster _currentCluster;

    public static Item CreateItem(string parentPath)
    {
        return new GenericContainerItem(parentPath, "container-instances",
            "Navigate the container instances within the ecs cluster");
    }
    
    public ContainerInstancesHandler(string path, IPathHandlerContext context, IAmazonECS ecs, CurrentCluster currentCluster) : base(path, context)
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
        return _ecs.QueryContainerInstances(_currentCluster.Name).Select(i => new ContainerInstanceItem(Path, i));
    }

    private static Regex _containerInstanceIdRegex = new(@"^[a-z0-9\*]+$");
    public override IEnumerable<Item> GetChildItems(string filter)
    {
        // ECS api can't find container instance by partial id. So defer to base implementation that relies on scanning all children
        if (_containerInstanceIdRegex.IsMatch(filter))
        {
            return base.GetChildItems(filter);
        }
        
        return _ecs.QueryContainerInstances(_currentCluster.Name, filter)
            .Select(i => new ContainerInstanceItem(Path, i));
    }
}