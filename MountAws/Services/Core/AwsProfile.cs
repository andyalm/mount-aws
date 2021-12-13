using System.Management.Automation;
using Amazon.Runtime.CredentialManagement;
using MountAnything;

namespace MountAws;

public class AwsProfile : Item
{
    private readonly CredentialProfile _profile;

    public AwsProfile(CredentialProfile profile) : base(string.Empty)
    {
        _profile = profile;
    }
    public override string ItemName => _profile.Name;
    public override object UnderlyingObject => _profile;
    public override string ItemType => "Profile";
    public override bool IsContainer => true;
}