using System.Management.Automation;
using Amazon.DynamoDBv2;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.DynamoDb;

public class TableItemsHandler(ItemPath path, IPathHandlerContext context, IAmazonDynamoDB dynamo) : PathHandler(path, context), IGetChildItemParameters<TableItemsChildItemParameters>
{
    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "items", "Navigate the items in this table");
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        var table = dynamo.DescribeTable(ItemName);

        return dynamo.Scan(ItemName, GetChildItemParameters.Limit).Select(v => new DynamoItem(Path, table.KeySchema, v));
    }

    // since we don't return the full child item set, we don't want the list of children cached
    protected override bool CacheChildren => false;

    public TableItemsChildItemParameters GetChildItemParameters { get; set; } = null!;
}

public class TableItemsChildItemParameters
{
    [Parameter]
    public int? Limit { get; set; }
}