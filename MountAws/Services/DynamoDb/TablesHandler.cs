using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using MountAnything;
using MountAws.Services.Core;

namespace MountAws.Services.DynamoDb;

public class TablesHandler : PathHandler
{
    private readonly IAmazonDynamoDB _dynamo;

    public static IItem CreateItem(ItemPath parentPath)
    {
        return new GenericContainerItem(parentPath, "tables",
            "Navigate the dynamo db tables as a virtual filesystem");
    }
    
    public TablesHandler(ItemPath path, IPathHandlerContext context, IAmazonDynamoDB dynamo) : base(path, context)
    {
        _dynamo = dynamo;
    }

    protected override IItem? GetItemImpl()
    {
        return CreateItem(ParentPath);
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        return _dynamo.ListTables().Select(tableName => new TableItem(Path, tableName));
    }
}