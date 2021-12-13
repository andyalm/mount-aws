using MountAnything;

namespace MountAws;

public class ProfilesRoot : Item
{
    public ProfilesRoot() : base(string.Empty)
    {
        
    }
    public override string ItemName => "";
    public override object UnderlyingObject => new();
    public override string ItemType => "Root";
    public override bool IsContainer => true;
}