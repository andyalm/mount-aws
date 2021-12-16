using MountAnything;
using Task = Amazon.ECS.Model.Task;

namespace MountAws.Services.ECS;

public class TaskItem : Item
{
    private readonly Task _task;

    public TaskItem(string parentPath, Task task) : base(parentPath)
    {
        _task = task;
    }

    public override string ItemName => _task.TaskArn.Split("/").Last();
    public override object UnderlyingObject => _task;
    public override string ItemType => "Task";
    public override bool IsContainer => false;
}