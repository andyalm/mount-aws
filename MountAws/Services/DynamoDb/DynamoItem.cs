using System.Management.Automation;
using Amazon.DynamoDBv2.Model;
using MountAnything;

namespace MountAws.Services.DynamoDb;

public class DynamoItem : AwsItem
{
    public DynamoItem(ItemPath parentPath, List<KeySchemaElement> keySchema, PSObject item) : base(parentPath, item)
    {
        ItemName = string.Join(",", keySchema.Select(s => item.Property<string>(s.AttributeName)));
    }

    public override string ItemName { get; }

    public override bool IsContainer => false;
}

