using Amazon;
using MountAnything;

namespace MountAws;

public class AwsRegion : Item
{
    private readonly RegionEndpoint _regionEndpoint;

    public AwsRegion(string parentPath, RegionEndpoint regionEndpoint) :  base(parentPath)
    {
        _regionEndpoint = regionEndpoint;
    }

    public override string ItemName => _regionEndpoint.SystemName;
    public override object UnderlyingObject => _regionEndpoint;
    public override string ItemType => "Region";
    public override bool IsContainer => true;
}