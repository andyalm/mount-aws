using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Ecs;

public class TaskFamilyItem : AwsItem
{
    public TaskFamilyItem(ItemPath parentPath, string family) : base(parentPath, new PSObject())
    {
        ItemName = family;
    }

    public override string ItemName { get; }
    public override bool IsContainer => true;
}