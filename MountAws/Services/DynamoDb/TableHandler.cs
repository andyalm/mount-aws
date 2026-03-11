using System.Management.Automation;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using MountAnything;

namespace MountAws.Services.DynamoDb;

public class TableHandler : PathHandler
{
    private readonly IAmazonDynamoDB _dynamo;

    public TableHandler(ItemPath path, IPathHandlerContext context, IAmazonDynamoDB dynamo) : base(path, context)
    {
        _dynamo = dynamo;
    }

    protected override IItem? GetItemImpl()
    {
        try
        {
            var table = _dynamo.DescribeTable(ItemName);
            return new TableItem(ParentPath, table);
        }
        catch (ResourceNotFoundException)
        {
            return null;
        }
    }

    protected override IEnumerable<IItem> GetChildItemsImpl()
    {
        yield return TableAutoscalingHandler.CreateItem(Path);
        yield return TableItemsHandler.CreateItem(Path);
    }
}