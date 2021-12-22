using System.Management.Automation;
using Amazon;
using Amazon.Runtime.CredentialManagement;

namespace MountAws.Api.AwsSdk;
public class AwsSdkCoreApi : ICoreApi
{
    private readonly CredentialProfileStoreChain _profileStore = new();
    
    public IEnumerable<PSObject> ListProfiles()
    {
        return _profileStore.ListProfiles().ToPSObjects();
    }

    public bool TryGetProfile(string profileName, out PSObject profile)
    {
        if (_profileStore.TryGetProfile(profileName, out var credentialProfile))
        {
            profile = credentialProfile.ToPSObject();
            return true;
        }

        profile = default!;
        return false;
    }

    public IEnumerable<PSObject> ListRegions()
    {
        return RegionEndpoint.EnumerableAllRegions.ToPSObjects();
    }

    public bool TryGetRegion(string regionName, out PSObject region)
    {
        var regionEndpoint = RegionEndpoint.GetBySystemName(regionName);
        if (regionEndpoint != null)
        {
            region = regionEndpoint.ToPSObject();
            return true;
        }

        region = default!;
        return false;
    }
}
