using Amazon.Lambda;
using Amazon.Lambda.Model;
using MountAnything;

namespace MountAws.Services.Lambda;

public class LayerVersionHandler : PathHandler
{
    private readonly IAmazonLambda _lambda;
    private readonly CurrentLayer _currentLayer;

    public LayerVersionHandler(ItemPath path, IPathHandlerContext context,
        IAmazonLambda lambda, CurrentLayer currentLayer)
        : base(path, context)
    {
        _lambda = lambda;
        _currentLayer = currentLayer;
    }

    protected override IItem? GetItemImpl()
    {
        try
        {
            if (!long.TryParse(ItemName, out var versionNumber))
            {
                return null;
            }

            var response = _lambda.GetLayerVersion(_currentLayer.Name, versionNumber);
            var layerVersion = new LayerVersionsListItem
            {
                LayerVersionArn = response.LayerVersionArn,
                Version = response.Version,
                Description = response.Description,
                CreatedDate = response.CreatedDate,
                CompatibleRuntimes = response.CompatibleRuntimes,
                LicenseInfo = response.LicenseInfo
            };
            return new LayerVersionItem(ParentPath, layerVersion, _currentLayer.Name);
        }
        catch (ResourceNotFoundException)
        {
            return null;
        }
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}
