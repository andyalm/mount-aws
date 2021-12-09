using System.Management.Automation;

namespace MountAws;

public class GenericContainerItem : AwsItem
{
    private readonly string _parentPath;

    public GenericContainerItem(string parentPath, string name, string itemType = "Container")
    {
        _parentPath = parentPath;
        ItemName = name;
        UnderlyingObject = new PSObject();
        ItemType = itemType;
    }

    public override string FullPath => AwsPath.Combine(_parentPath, ItemName);
    public override string ItemName { get; }
    public override object UnderlyingObject { get; }
    public override string ItemType { get; }
    public override bool IsContainer => true;
}