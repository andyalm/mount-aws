using Amazon.DynamoDBv2;
using MountAnything;

namespace MountAws.Services.DynamoDb;

public class ItemHandler : PathHandler
{
    private readonly IAmazonDynamoDB _dynamo;

    public ItemHandler(ItemPath path, IPathHandlerContext context, IAmazonDynamoDB dynamo) : base(path, context)
    {
        _dynamo = dynamo;
    }

    protected override IItem? GetItemImpl()
    {
        var table = _dynamo.DescribeTable(ParentPath.Name);
        var keys = ItemName.Split(",");
        var item = _dynamo.GetItem(table, keys);

        return new DynamoItem(ParentPath, table.KeySchema, item);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}