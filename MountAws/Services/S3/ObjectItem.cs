using System.Management.Automation;
using MountAnything;
using MountAws.Api;

namespace MountAws.Services.S3;

public class ObjectItem : AwsItem
{
    public ObjectItem(string parentPath, PSObject s3Object) : base(parentPath, s3Object)
    {
        ItemName = ItemPath.GetLeaf(s3Object.Property<string>("Key")!);
        ItemType = "File";
        IsContainer = false;
    }
    
    public ObjectItem(string parentPath, string prefix) : base(parentPath, new PSObject())
    {
        ItemName = ItemPath.GetLeaf(prefix.TrimEnd('/'));
        ItemType = "Directory";
        IsContainer = true;
    }

    public override string ItemName { get; }
    public override string ItemType { get; }
    public override string TypeName => "MountAws.Services.S3.ObjectItem";
    public override bool IsContainer { get; }
}