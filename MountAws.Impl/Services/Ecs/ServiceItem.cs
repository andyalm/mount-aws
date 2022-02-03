using Amazon.ECS.Model;
using MountAnything;
using MountAws.Services.Elbv2;

namespace MountAws.Services.Ecs;

public class ServiceItem : AwsItem<Service>
{
    public ServiceItem(ItemPath parentPath, Service service, LinkGenerator linkGenerator) : base(parentPath, service)
    {
        ItemName = service.ServiceName;
        LinkPaths = new Dictionary<string,ItemPath>
        {
            ["TaskDefinition"] = linkGenerator.TaskDefinition(service.TaskDefinition)
        };
        var loadBalancer = service.LoadBalancers.FirstOrDefault(l => l.TargetGroupArn != null);
        if (loadBalancer != null)
        {
            LinkPaths["TargetGroup"] = linkGenerator.TargetGroup(loadBalancer.TargetGroupArn);
        }
    }

    public override string ItemName { get; }
    public override string ItemType => EcsItemTypes.Service;
    public override string? WebUrl =>
        UrlBuilder.CombineWith(
            $"ecs/home#/clusters/{UnderlyingObject.ClusterName()}/services/{ItemName}");
    public override bool IsContainer => true;
}