using System.Management.Automation;
using Amazon.Runtime.CredentialManagement;

namespace MountAws;

public class AwsProfile : AwsItem
{
    private readonly CredentialProfile _profile;

    public AwsProfile(CredentialProfile profile)
    {
        _profile = profile;
    }

    public override string FullPath => _profile.Name;
    public override string ItemName => _profile.Name;
    public override object UnderlyingObject => _profile;
    public override string ItemType => "Profile";
    public override bool IsContainer => true;
}