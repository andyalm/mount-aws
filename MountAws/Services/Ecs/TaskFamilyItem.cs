using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Ecs;

public class TaskFamilyItem : Item
{
    public TaskFamilyItem(string parentPath, string family) : base(parentPath, new PSObject())
    {
        ItemName = family;
    }

    public override string ItemName { get; }
    public override bool IsContainer => true;
    public override string TypeName => GetType().FullName!;
}