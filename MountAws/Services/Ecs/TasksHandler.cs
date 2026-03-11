using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.ECS;
using MountAnything;
using MountAws.Api.AwsSdk.Ecs;
using MountAws.Services.Core;
using MountAws.Services.Ec2;

namespace MountAws.Services.Ecs;

public class TasksHandler(
    ItemPath path,
    IPathHandlerContext context,
    CurrentCluster currentCluster,
    CurrentService currentService,
    IAmazonECS ecs,
    IAmazonEC2 ec2) : PathHandler(path, context)
{
    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "tasks",
            "Navigate the ECS tasks in this service");
    }
    
    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var taskArns = ecs.ListTasksByService(currentCluster.Name, currentService.Name);

        var tasks = taskArns.Chunk(10).SelectMany(taskArnChunk =>
        {
            return ecs.DescribeTasks(currentCluster.Name,
                taskArnChunk,
                new[] { "TAGS" });
        });
        
        var containerInstanceArns = tasks
            .Where(t => !string.IsNullOrEmpty(t.ContainerInstanceArn))
            .Select(t => t.ContainerInstanceArn)
            .Distinct()
            .ToArray();
        Dictionary<string, Instance> ec2InstancesByContainerInstanceArn = new Dictionary<string, Instance>();
        if (containerInstanceArns.Any())
        {
            var containerInstances = ecs.DescribeContainerInstances(currentCluster.Name, containerInstanceArns)
                .Where(c => !string.IsNullOrEmpty(c.Ec2InstanceId))
                .ToDictionary(i => i.Ec2InstanceId);

            var ec2InstanceIds = containerInstances.Values.Select(c => c.Ec2InstanceId);
            ec2InstancesByContainerInstanceArn = ec2.GetInstancesByIds(ec2InstanceIds)
                .Where(instance => containerInstances.ContainsKey(instance.InstanceId))
                .ToDictionary(i => containerInstances[i.InstanceId].ContainerInstanceArn);
        }

        return tasks.Select(t => new TaskItem(Path, t,
            t.ContainerInstanceArn != null
                ? ec2InstancesByContainerInstanceArn.GetValueOrDefault(t.ContainerInstanceArn)
                : null, LinkGenerator, useServiceView:true));
    }
}