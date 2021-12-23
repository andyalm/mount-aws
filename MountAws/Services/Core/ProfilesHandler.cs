using MountAnything;
using MountAws.Api;

namespace MountAws;

public class ProfilesHandler : PathHandler
{
    private readonly ICoreApi _coreApi;

    public ProfilesHandler(string path, IPathHandlerContext context, ICoreApi coreApi) : base(path, context)
    {
        _coreApi = coreApi;
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
        return _coreApi.ListProfiles().Select(p => new ProfileItem(p));
    }
}