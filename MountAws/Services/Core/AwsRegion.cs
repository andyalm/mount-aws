using Amazon;

namespace MountAws;

public class AwsRegion : AwsItem
{
    private readonly string _parentPath;
    private readonly RegionEndpoint _regionEndpoint;

    public AwsRegion(string parentPath, RegionEndpoint regionEndpoint)
    {
        _parentPath = parentPath;
        _regionEndpoint = regionEndpoint;
    }

    public override string FullPath => AwsPath.Combine(_parentPath, Name);
    public override string Name => _regionEndpoint.SystemName;
    public override object UnderlyingObject => _regionEndpoint;
    public override string ItemType => "Region";
    public override bool IsContainer => true;
}