using Amazon.Lambda.Model;
using MountAnything;

namespace MountAws.Services.Lambda;

public class LayerItem : AwsItem<LayersListItem>
{
    public LayerItem(ItemPath parentPath, LayersListItem layer)
        : base(parentPath, layer) { }

    public override string ItemName => UnderlyingObject.LayerName;
    public override string ItemType => LambdaItemTypes.Layer;
    public override bool IsContainer => true;
    public override string? WebUrl => UrlBuilder.CombineWith($"lambda/home#/layers/{ItemName}");
}
