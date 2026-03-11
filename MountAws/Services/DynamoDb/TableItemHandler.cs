using Amazon.DynamoDBv2;
using MountAnything;

namespace MountAws.Services.DynamoDb;

public class TableItemHandler(
    ItemPath path,
    IPathHandlerContext context,
    CurrentTable currentTable,
    IAmazonDynamoDB dynamo)
    : PathHandler(path, context)
{
    protected override IItem? GetItemImpl()
    {
        var table = dynamo.DescribeTable(currentTable.Name);
        var keys = ItemName.Split(",");
        var item = dynamo.GetItem(table, keys);

        return new DynamoItem(ParentPath, table.KeySchema, item);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield break;
    }
}