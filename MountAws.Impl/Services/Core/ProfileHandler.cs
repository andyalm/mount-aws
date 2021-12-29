using MountAnything;
using MountAws.Api;

namespace MountAws;

public class ProfileHandler : PathHandler
{
    private readonly ICoreApi _api;
    private readonly CurrentProfile _currentProfile;

    public ProfileHandler(string path, IPathHandlerContext context, ICoreApi api, CurrentProfile currentProfile) : base(path, context)
    {
        _api = api;
        _currentProfile = currentProfile;
    }

    protected override IItem? GetItemImpl()
    {
        if(_api.TryGetProfile(_currentProfile.Value, out var profile))
        {
            return new ProfileItem(profile);
        }

        return null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _api.ListRegions().Select(r => new RegionItem(Path, r));
    }
}