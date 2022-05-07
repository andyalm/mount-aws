using System.Management.Automation;
using Amazon.DynamoDBv2.Model;
using MountAnything;

namespace MountAws.Services.DynamoDb;

public class TableItem : AwsItem
{
    public TableItem(ItemPath parentPath, string tableName) : base(parentPath, new PSObject(new
    {
        TableName = tableName
    }))
    {
        ItemName = tableName;
    }
    
    public TableItem(ItemPath parentPath, TableDescription table) : base(parentPath, new PSObject(table))
    {
        ItemName = table.TableName;
    }

    public override string ItemName { get; }

    public override string? WebUrl => UrlBuilder.CombineWith($"dynamodbv2/home#table?initialTagKey=&name={ItemName}");

    public override bool IsContainer => true;
}