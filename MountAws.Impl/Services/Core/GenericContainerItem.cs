using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Core;

public class GenericContainerItem : Item
{
    public GenericContainerItem(ItemPath parentPath, string name, string description, string itemType = "Container") 
        : base(parentPath, new PSObject(new
        {
            Name = name,
            Description = description
        }))
    {
        ItemName = name;
        Description = description;
        ItemType = itemType;
    }

    public override string ItemName { get; }
    public override string ItemType { get; }
    public string Description { get; }
    public override bool IsContainer => true;
    public override string TypeName => GetType().FullName!;
}