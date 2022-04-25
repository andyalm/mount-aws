using Amazon.WAFV2.Model;
using MountAnything;

namespace MountAws.Services.Wafv2;

public class CustomHeaderItem : AwsItem<CustomHTTPHeader>
{
    public CustomHeaderItem(ItemPath parentPath, CustomHTTPHeader underlyingObject) : base(parentPath, underlyingObject)
    {
    }

    public override string ItemName => UnderlyingObject.Name;
    public override bool IsContainer => false;
}