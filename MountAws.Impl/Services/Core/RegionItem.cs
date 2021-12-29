using Amazon;

namespace MountAws.Services.Core;

public class RegionItem : AwsItem<RegionEndpoint>
{
    public RegionItem(string parentPath, RegionEndpoint regionEndpoint) :  base(parentPath, regionEndpoint)
    {
        
    }

    public override string ItemName => UnderlyingObject.SystemName;
    public override string ItemType => CoreItemTypes.Region;
    public override bool IsContainer => true;
}