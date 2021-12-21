using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.S3;

public class PolicyItem : Item
{
    public PolicyItem(string parentPath) : base(parentPath)
    {
        UnderlyingObject = new { };
    }

    public override string ItemName => "policy";
    public override object UnderlyingObject { get; }
    public override string ItemType => "BucketPolicy";

    public override string TypeName => typeof(GenericContainerItem).FullName!;
    public override bool IsContainer => false;
}