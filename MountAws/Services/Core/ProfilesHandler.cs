using Amazon.Runtime.CredentialManagement;

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

    protected override AwsItem? GetItemImpl()
    {
        return new ProfilesRoot();
    }

    protected override IEnumerable<AwsItem> GetChildItemsImpl()
    {
        var c = new CredentialProfileStoreChain();
        return c.ListProfiles().Select(p => new AwsProfile(p));
    }
}