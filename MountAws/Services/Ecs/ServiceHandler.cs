using Amazon.EC2;
using Amazon.EC2.Model;
using Amazon.ECS;
using MountAnything;
using MountAws.Api.AwsSdk.Ecs;
using MountAws.Services.AppAutoscaling;
using MountAws.Services.Ec2;

namespace MountAws.Services.Ecs;

public class ServiceHandler : PathHandler, IRemoveItemHandler
{
    private readonly IAmazonECS _ecs;
    private readonly IAmazonEC2 _ec2;
    private readonly CurrentCluster _currentCluster;

    public ServiceHandler(ItemPath path, IPathHandlerContext context, IAmazonECS ecs, IAmazonEC2 ec2, CurrentCluster currentCluster) : base(path, context)
    {
        _ecs = ecs;
        _ec2 = ec2;
        _currentCluster = currentCluster;
    }

    protected override IItem? GetItemImpl()
    {
        var service = _ecs.DescribeServices(_currentCluster.Name,
            new[] { ItemName },
            new[] { "TAGS" }).FirstOrDefault();
        
        if (service != null)
        {
            return new ServiceItem(ParentPath, service, LinkGenerator);
        }

        return null;
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return TasksHandler.CreateItem(Path);
        yield return AutoscalingHandler.CreateItem(Path);
    }

    public void RemoveItem()
    {
        _ecs.DeleteService(_currentCluster.Name, ItemName, Context.Force);
    }
}