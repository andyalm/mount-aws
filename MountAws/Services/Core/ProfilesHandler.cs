using Amazon.Runtime.CredentialManagement;
using MountAnything;

namespace MountAws;

public class ProfilesHandler : PathHandler
{
    public ProfilesHandler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override bool ExistsImpl()
    {
        return true;
    }

    protected override Item? GetItemImpl()
    {
        return new ProfilesRoot();
    }

    protected override IEnumerable<Item> GetChildItemsImpl()
    {
        var c = new CredentialProfileStoreChain();
        return c.ListProfiles().Select(p => new AwsProfile(p));
    }
}