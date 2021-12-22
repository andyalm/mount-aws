using System.Management.Automation;
using Amazon.Runtime.CredentialManagement;
using MountAnything;

namespace MountAws;

public class ProfileItem : AwsItem
{
    public ProfileItem(PSObject profile) : base(string.Empty, profile)
    {
        
    }
    public override string ItemName => Property<string>("Name")!;
    public override string ItemType => "Profile";
    public override bool IsContainer => true;
}