using Amazon;
using Amazon.Runtime.CredentialManagement;

namespace MountAws;

public class ProfileHandler : PathHandler
{
    private readonly CredentialProfile _profile;

    public ProfileHandler(string path, IPathHandlerContext context, CredentialProfile profile) : base(path, context)
    {
        _profile = profile;
    }

    protected override bool ExistsImpl()
    {
        return GetItem() != null;
    }

    protected override AwsItem? GetItemImpl()
    {
        return new AwsProfile(_profile);
    }

    protected override IEnumerable<AwsItem> GetChildItemsImpl()
    {
        return RegionEndpoint.EnumerableAllRegions.Select(e => new AwsRegion(Path, e));
    }
}