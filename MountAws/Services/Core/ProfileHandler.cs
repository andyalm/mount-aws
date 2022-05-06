using Amazon;
using Amazon.Runtime.CredentialManagement;
using MountAnything;
using MountAws.Api;
using MountAws.Services.Core;

namespace MountAws;

public class ProfileHandler : PathHandler
{
    private readonly CredentialProfileStoreChain _credentialChain;

    public ProfileHandler(ItemPath path, IPathHandlerContext context) : base(path, context)
    {
        _credentialChain = new CredentialProfileStoreChain();
    }

    protected override IItem? GetItemImpl()
    {
        if(_credentialChain.TryGetProfile(ItemName, out var profile))
        {
            return new ProfileItem(profile);
        }

        return null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return RegionEndpoint.EnumerableAllRegions.Select(r => new RegionItem(Path, r));
    }
}