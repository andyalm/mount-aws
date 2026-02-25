using Amazon.Lambda.Model;
using MountAnything;

namespace MountAws.Services.Lambda;

public class LayerVersionItem : AwsItem<LayerVersionsListItem>
{
    private readonly string _layerName;

    public LayerVersionItem(ItemPath parentPath, LayerVersionsListItem layerVersion, string layerName)
        : base(parentPath, layerVersion)
    {
        _layerName = layerName;
    }

    public override string ItemName => UnderlyingObject.Version.ToString();
    public override string ItemType => LambdaItemTypes.LayerVersion;
    public override bool IsContainer => false;
    public override string? WebUrl =>
        UrlBuilder.CombineWith($"lambda/home#/layers/{_layerName}/versions/{ItemName}");
}
