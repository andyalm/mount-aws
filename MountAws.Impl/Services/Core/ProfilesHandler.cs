using Amazon.Runtime.CredentialManagement;
using MountAnything;
using MountAws.Api;
using MountAws.Services.Core;

namespace MountAws;

public class ProfilesHandler : PathHandler
{
    private readonly CredentialProfileStoreChain _credentialChain;
    
    public ProfilesHandler(string path, IPathHandlerContext context) : base(path, context)
    {
        _credentialChain = new CredentialProfileStoreChain();
    }

    protected override bool ExistsImpl()
    {
        return true;
    }

    protected override IItem? GetItemImpl()
    {
        return new ProfilesRoot();
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _credentialChain.ListProfiles().Select(p => new ProfileItem(p));
    }
}