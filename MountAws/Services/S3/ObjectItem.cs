using Amazon.S3.Model;
using MountAnything;

namespace MountAws.Services.S3;

public class ObjectItem : Item
{
    private readonly object _object;
    
    public ObjectItem(string parentPath, S3Object @object) : base(parentPath)
    {
        _object = @object;
        ItemName = ItemPath.GetLeaf(@object.Key);
        ItemType = "File";
        IsContainer = false;
    }
    
    public ObjectItem(string parentPath, GetObjectResponse response) : base(parentPath)
    {
        _object = response;
        ItemName = ItemPath.GetLeaf(response.Key);
        ItemType = "File";
        IsContainer = false;
    }
    
    public ObjectItem(string parentPath, string prefix) : base(parentPath)
    {
        _object = new { };
        ItemName = ItemPath.GetLeaf(prefix.TrimEnd('/'));
        ItemType = "Directory";
        IsContainer = true;
    }

    public override string ItemName { get; }
    public override object UnderlyingObject => _object;
    public override string ItemType { get; }
    public override string TypeName => "MountAws.Services.S3.ObjectItem";
    public override bool IsContainer { get; }
}