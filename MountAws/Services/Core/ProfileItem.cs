using System.Management.Automation;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws;

public class ProfileItem : AwsItem
{
    public ProfileItem(PSObject profile) : base(string.Empty, profile)
    {
        
    }
    public override string ItemName => Property<string>("Name")!;
    public override string ItemType => CoreItemTypes.Profile;
    public override bool IsContainer => true;
}