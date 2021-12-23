using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.S3;

public class BucketItem : Item
{
    public BucketItem(string parentPath, string bucketName) : base(parentPath, new PSObject(new
    {
        BucketName = bucketName
    }))
    {
        ItemName = bucketName;
    }

    public override string ItemName { get; }
    public override string TypeName => "MountAws.Services.S3.BucketItem";
    public override string ItemType => S3ItemTypes.Bucket;
    public override bool IsContainer => true;
}