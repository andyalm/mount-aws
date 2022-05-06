using System.Management.Automation;
using Amazon.S3.Model;
using MountAnything;

namespace MountAws.Services.S3;

public class ObjectItem : AwsItem
{
    public ObjectItem(ItemPath parentPath, GetObjectResponse s3Object) : base(parentPath, new PSObject(s3Object))
    {
        Key = s3Object.Key;
        ItemName = new ItemPath(s3Object.Key).Name;
        ItemType = S3ItemTypes.File;
        IsContainer = false;
        BucketName = s3Object.BucketName;
    }
    
    public ObjectItem(ItemPath parentPath, S3Object s3Object) : base(parentPath, new PSObject(s3Object))
    {
        Key = s3Object.Key;
        ItemName = new ItemPath(s3Object.Key).Name;
        ItemType = S3ItemTypes.File;
        IsContainer = false;
        BucketName = s3Object.BucketName;
    }
    
    public ObjectItem(ItemPath parentPath, string prefix, string bucketName) : base(parentPath, new PSObject())
    {
        Key = prefix;
        ItemName = new ItemPath(prefix.TrimEnd('/')).Name;
        ItemType = S3ItemTypes.Directory;
        IsContainer = true;
        BucketName = bucketName;
    }

    public override string ItemName { get; }
    public override string ItemType { get; }
    public override bool IsContainer { get; }
    
    [ItemProperty]
    public string Key { get; }
    
    [ItemProperty]
    public string BucketName { get; }

    public override string? WebUrl => WebUrlBuilder.S3().CombineWith($"s3/object/{BucketName}?prefix={Key}");
}