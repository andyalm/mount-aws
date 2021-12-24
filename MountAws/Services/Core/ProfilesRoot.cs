using System.Management.Automation;
using MountAnything;

namespace MountAws;

public class ProfilesRoot : Item
{
    public ProfilesRoot() : base(string.Empty, new PSObject())
    {
        
    }
    public override string ItemName => "";
    public override string ItemType => "Root";
    public override bool IsContainer => true;
}