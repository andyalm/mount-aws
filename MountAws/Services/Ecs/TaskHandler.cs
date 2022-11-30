using System.Management.Automation;
using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.ECS;
using MountAnything;
using MountAws.Api.AwsSdk.Ecs;
using MountAws.Services.Ec2;

namespace MountAws.Services.Ecs;

public class TaskHandler : PathHandler, IRemoveItemHandler, IRemoveItemParameters<RemoveTaskParameters>
{
    private readonly IAmazonECS _ecs;
    private readonly IAmazonEC2 _ec2;
    private readonly CurrentCluster _currentCluster;

    public TaskHandler(ItemPath path, IPathHandlerContext context, IAmazonECS ecs, IAmazonEC2 ec2, CurrentCluster currentCluster) : base(path, context)
    {
        _ecs = ecs;
        _ec2 = ec2;
        _currentCluster = currentCluster;
    }

    protected override IItem? GetItemImpl()
    {
        var task = _ecs.DescribeTasks(_currentCluster.Name,
            new[] { ItemName },
            new[] { "TAGS" }).SingleOrDefault();

        if (task != null)
        {
            Instance? ec2Instance = null;
            if (!string.IsNullOrEmpty(task.ContainerInstanceArn))
            {
                try
                {
                    var containerInstance =
                        _ecs.DescribeContainerInstance(_currentCluster.Name, task.ContainerInstanceArn);
                    if (!string.IsNullOrEmpty(containerInstance.Ec2InstanceId))
                    {
                        ec2Instance = _ec2.GetInstancesByIds(new[] { containerInstance.Ec2InstanceId })
                            .FirstOrDefault();
                    }
                }
                catch (ContainerInstanceNotFoundException ex)
                {
                    Context.WriteDebug(ex.ToString());
                }
            }
            return new TaskItem(ParentPath, task, ec2Instance, LinkGenerator, useServiceView: ParentPath.FullName.Contains("/services/"));
        }

        return null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }

    public void RemoveItem()
    {
        _ecs.StopTask(_currentCluster.Name, ItemName, RemoveItemParameters?.Reason);
    }

    public RemoveTaskParameters? RemoveItemParameters { get; set; }
}

public class RemoveTaskParameters
{
    [Parameter(Mandatory = false)]
    public string? Reason { get; set; }
}