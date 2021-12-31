using System.Management.Automation;
using Amazon.S3.Model;
using MountAnything;

namespace MountAws.Services.S3;

public class ObjectItem : AwsItem
{
    public ObjectItem(ItemPath parentPath, GetObjectResponse s3Object) : base(parentPath, new PSObject(s3Object))
    {
        ItemName = new ItemPath(s3Object.Key).Name;
        ItemType = S3ItemTypes.File;
        IsContainer = false;
    }
    
    public ObjectItem(ItemPath parentPath, S3Object s3Object) : base(parentPath, new PSObject(s3Object))
    {
        ItemName = new ItemPath(s3Object.Key).Name;
        ItemType = S3ItemTypes.File;
        IsContainer = false;
    }
    
    public ObjectItem(ItemPath parentPath, string prefix) : base(parentPath, new PSObject())
    {
        ItemName = new ItemPath(prefix.TrimEnd('/')).Name;
        ItemType = S3ItemTypes.Directory;
        IsContainer = true;
    }

    public override string ItemName { get; }
    public override string ItemType { get; }
    public override bool IsContainer { get; }
}