using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.DynamoDb;

public class DynamoDbRootHandler : PathHandler
{
    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "dynamodb",
            "Navigate dynamo db tables is a virtual filesystem");
    }
    
    public DynamoDbRootHandler(ItemPath path, IPathHandlerContext context) : base(path, context)
    {
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return TablesHandler.CreateItem(Path);
    }
}