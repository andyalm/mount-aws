using System.Management.Automation;
using MountAnything;

namespace MountAws;

public class ProfileItem : Item
{
    public ProfileItem(PSObject profile) : base(string.Empty, profile)
    {
        
    }
    public override string ItemName => Property<string>("Name")!;
    public override string ItemType => "Profile";
    public override bool IsContainer => true;
}