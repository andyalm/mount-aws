using Amazon.ApplicationAutoScaling;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.AppAutoscaling;

public class ServicesHandler : PathHandler
{
    internal static readonly ServiceNamespace[] KnownServiceNamespaces =
    [
        ServiceNamespace.Appstream,
        ServiceNamespace.Cassandra,
        ServiceNamespace.Comprehend,
        ServiceNamespace.CustomResource,
        ServiceNamespace.Dynamodb,
        ServiceNamespace.Ec2,
        ServiceNamespace.Ecs,
        ServiceNamespace.Elasticache,
        ServiceNamespace.Elasticmapreduce,
        ServiceNamespace.Kafka,
        ServiceNamespace.Lambda,
        ServiceNamespace.Neptune,
        ServiceNamespace.Rds,
        ServiceNamespace.Sagemaker,
        ServiceNamespace.Workspaces
    ];

    public static Item CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "services",
            "Navigate application autoscaling service namespaces");
    }

    public ServicesHandler(ItemPath path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return KnownServiceNamespaces.Select(ns => new ServiceItem(Path, ns));
    }
}
