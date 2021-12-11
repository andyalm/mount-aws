using System.Management.Automation;

namespace MountAws.Services.Core;

public class GenericContainerItem : AwsItem
{
    private readonly string _parentPath;

    public GenericContainerItem(string parentPath, string name, string description, string itemType = "Container")
    {
        _parentPath = parentPath;
        ItemName = name;
        Description = description;
        UnderlyingObject = new PSObject(new
        {
            Name = name,
            Description = description
        });
        ItemType = itemType;
    }

    public override string FullPath => AwsPath.Combine(_parentPath, ItemName);
    public override string ItemName { get; }
    public override object UnderlyingObject { get; }
    public override string ItemType { get; }
    public string Description { get; }
    public override bool IsContainer => true;
    public override string TypeName => GetType().FullName!;
}