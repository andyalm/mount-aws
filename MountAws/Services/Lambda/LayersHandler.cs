using Amazon.Lambda;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.Lambda;

public class LayersHandler : PathHandler
{
    private readonly IAmazonLambda _lambda;

    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "layers",
            "Navigate Lambda layers in this account and region");
    }

    public LayersHandler(ItemPath path, IPathHandlerContext context, IAmazonLambda lambda)
        : base(path, context)
    {
        _lambda = lambda;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _lambda.ListLayers()
            .Select(l => new LayerItem(Path, l))
            .OrderBy(l => l.ItemName);
    }
}
