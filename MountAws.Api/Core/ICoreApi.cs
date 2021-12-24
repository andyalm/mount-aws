using System.Management.Automation;

namespace MountAws.Api;
public interface ICoreApi
{
    IEnumerable<PSObject> ListProfiles();
    bool TryGetProfile(string profileName, out PSObject profile);

    IEnumerable<PSObject> ListRegions();

    bool TryGetRegion(string regionName, out PSObject region);
}

public static class CoreApiExtensions
{
    public static PSObject GetRequiredProfile(this ICoreApi api, string profileName)
    {
        if (api.TryGetProfile(profileName, out var profile))
        {
            return profile;
        }

        throw new ProfileNotFoundException(profileName);
    }

    public static PSObject GetRequiredRegion(this ICoreApi api, string regionName)
    {
        if (api.TryGetRegion(regionName, out var region))
        {
            return region;
        }

        throw new RegionNotFoundException(regionName);
    }
}
