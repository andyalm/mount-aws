using System.Management.Automation;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using MountAnything;

namespace MountAws.Services.DynamoDb;

public class TableHandler : PathHandler, IGetChildItemParameters<TableChildItemParameters>
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
        var table = _dynamo.DescribeTable(ItemName);

        return _dynamo.Scan(ItemName, GetChildItemParameters.Limit).Select(v => new DynamoItem(Path, table.KeySchema, v));
    }

    // since we don't return the full child item set, we don't want the list of children cached
    protected override bool CacheChildren => false;

    public TableChildItemParameters GetChildItemParameters { get; set; } = null!;
}

public class TableChildItemParameters
{
    [Parameter()]
    public int? Limit { get; set; }
}