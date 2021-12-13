using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Core;

public class GenericContainerItem : Item
{
    public GenericContainerItem(string parentPath, string name, string description, string itemType = "Container") : base(parentPath)
    {
        ItemName = name;
        Description = description;
        UnderlyingObject = new PSObject(new
        {
            Name = name,
            Description = description
        });
        ItemType = itemType;
    }

    public override string ItemName { get; }
    public override object UnderlyingObject { get; }
    public override string ItemType { get; }
    public string Description { get; }
    public override bool IsContainer => true;
    public override string TypeName => GetType().FullName!;
}