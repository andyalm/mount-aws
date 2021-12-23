using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Ecs;

public class TaskItem : Item
{

    public TaskItem(string parentPath, PSObject task) : base(parentPath, task)
    {
        ItemName = Property<string>("TaskArn")!.Split("/").Last();
    }

    public override string ItemName { get; }
    public override string ItemType => EcsItemTypes.Task;
    public override bool IsContainer => false;
}