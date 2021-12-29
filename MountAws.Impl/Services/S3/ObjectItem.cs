using System.Management.Automation;
using Amazon.S3.Model;
using MountAnything;

namespace MountAws.Services.S3;

public class ObjectItem : AwsItem
{
    public ObjectItem(string parentPath, GetObjectResponse s3Object) : base(parentPath, new PSObject(s3Object))
    {
        ItemName = ItemPath.GetLeaf(s3Object.Key);
        ItemType = S3ItemTypes.File;
        IsContainer = false;
    }
    
    public ObjectItem(string parentPath, S3Object s3Object) : base(parentPath, new PSObject(s3Object))
    {
        ItemName = ItemPath.GetLeaf(s3Object.Key);
        ItemType = S3ItemTypes.File;
        IsContainer = false;
    }
    
    public ObjectItem(string parentPath, string prefix) : base(parentPath, new PSObject())
    {
        ItemName = ItemPath.GetLeaf(prefix.TrimEnd('/'));
        ItemType = S3ItemTypes.Directory;
        IsContainer = true;
    }

    public override string ItemName { get; }
    public override string ItemType { get; }
    public override bool IsContainer { get; }
}