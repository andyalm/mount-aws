using Amazon.Lambda;
using MountAnything;

namespace MountAws.Services.Lambda;

public class LayerHandler : PathHandler
{
    private readonly IAmazonLambda _lambda;
    private readonly CurrentLayer _currentLayer;

    public LayerHandler(ItemPath path, IPathHandlerContext context,
        IAmazonLambda lambda, CurrentLayer currentLayer)
        : base(path, context)
    {
        _lambda = lambda;
        _currentLayer = currentLayer;
    }

    protected override IItem? GetItemImpl()
    {
        var layer = _lambda.ListLayers()
            .FirstOrDefault(l => l.LayerName.Equals(ItemName, StringComparison.OrdinalIgnoreCase));

        if (layer == null)
        {
            return null;
        }

        return new LayerItem(ParentPath, layer);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _lambda.ListLayerVersions(_currentLayer.Name)
            .Select(v => new LayerVersionItem(Path, v, _currentLayer.Name))
            .OrderBy(v => v.ItemName);
    }
}
