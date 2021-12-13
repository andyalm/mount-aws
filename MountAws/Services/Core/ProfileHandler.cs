using Amazon;
using Amazon.Runtime.CredentialManagement;
using MountAnything;

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

    protected override Item? GetItemImpl()
    {
        return new AwsProfile(_profile);
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        return RegionEndpoint.EnumerableAllRegions.Select(e => new AwsRegion(Path, e));
    }
}