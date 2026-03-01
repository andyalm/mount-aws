using Amazon.ApplicationAutoScaling;
using Amazon.ApplicationAutoScaling.Model;
using MountAnything;
using MountAws.Services.Autoscaling;
using MountAws.Services.Core;

namespace MountAws.Services.DynamoDb;

public class TableAutoscalingHandler(ItemPath path, IPathHandlerContext context, IAmazonApplicationAutoScaling autoScaling) : PathHandler(path, context)
{
    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "autoscaling",
            "Navigate DynamoDB autoscaling dimensions, policies and activities for this table");
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var tableName = ParentPath.Name;
        return autoScaling.DescribeScalableTargets(ServiceNamespace.Dynamodb)
            .Where(t => t.ResourceId == $"table/{tableName}")
            .Select(t => new ScalableTargetItem(Path, t))
            .ToList();
    }
}
