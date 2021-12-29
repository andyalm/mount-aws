using System.Management.Automation;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.S3;

public class PolicyItem : AwsItem
{
    public PolicyItem(string parentPath) : base(parentPath, new PSObject()) {}

    public override string ItemName => "policy";
    public override string ItemType => S3ItemTypes.BucketPolicy;
    public override string TypeName => typeof(GenericContainerItem).FullName!;
    public override bool IsContainer => false;
}