using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.DynamoDb;

public class TableAutoscalingHandler(ItemPath path, IPathHandlerContext context) : PathHandler(path, context)
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
        throw new NotImplementedException();
    }
}