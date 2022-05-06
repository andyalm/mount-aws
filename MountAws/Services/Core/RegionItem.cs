using Amazon;
using MountAnything;

namespace MountAws.Services.Core;

public class RegionItem : AwsItem<RegionEndpoint>
{
    public RegionItem(ItemPath parentPath, RegionEndpoint regionEndpoint) :  base(parentPath, regionEndpoint)
    {
        
    }

    public override string ItemName => UnderlyingObject.SystemName;
    public override string ItemType => CoreItemTypes.Region;
    public override string? WebUrl => WebUrlBuilder.ForRegion(ItemName).CombineWith($"console/home?region={ItemName}");
    public override bool IsContainer => true;
}