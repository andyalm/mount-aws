using System.Management.Automation;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.S3;

public class PolicyItem : AwsItem
{
    public PolicyItem(ItemPath parentPath) : base(parentPath, new PSObject()) {}

    public override string ItemName => "policy";
    public override string ItemType => S3ItemTypes.BucketPolicy;
    protected override string TypeName => typeof(GenericContainerItem).FullName!;
    public override bool IsContainer => false;
    public override string? WebUrl => WebUrlBuilder.S3().CombineWith($"s3/buckets/{ItemName}?tab=permissions");
}