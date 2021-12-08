using Amazon.Runtime.CredentialManagement;

namespace MountAws;

public class ProfileHandler : PathHandler
{
    public ProfileHandler(string path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override bool ExistsImpl()
    {
        return GetItem() != null;
    }

    protected override AwsItem? GetItemImpl()
    {
        var c = new CredentialProfileStoreChain();
        if (c.TryGetProfile(ItemName, out var profile))
        {
            return new AwsProfile(profile);
        }

        return null;
    }

    protected override IEnumerable<AwsItem> GetChildItemsImpl()
    {
        return Enumerable.Empty<AwsItem>();
    }
}