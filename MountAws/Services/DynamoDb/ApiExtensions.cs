using System.Management.Automation;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using MountAnything;
using static MountAws.PagingHelper;

namespace MountAws.Services.DynamoDb;

public static class ApiExtensions
{
    public static IEnumerable<string> ListTables(this IAmazonDynamoDB dynamo)
    {
        return Paginate((nextToken) =>
        {
            var response = dynamo.ListTablesAsync(new ListTablesRequest
            {
                ExclusiveStartTableName = nextToken
            }).GetAwaiter().GetResult();

            return (response.TableNames, response.LastEvaluatedTableName);
        });
    }

    public static TableDescription DescribeTable(this IAmazonDynamoDB dynamo, string tableName)
    {
        return dynamo.DescribeTableAsync(new DescribeTableRequest
        {
            TableName = tableName
        }).GetAwaiter().GetResult().Table;
    }

    public static PSObject GetItem(this IAmazonDynamoDB dynamo, TableDescription table, string[] keyValues)
    {
        var keys = ToKeys(table.KeySchema, keyValues);
        return dynamo.GetItemAsync(new GetItemRequest
        {
            TableName = table.TableName,
            Key = keys
        }).GetAwaiter().GetResult().Item.ToPSObject();
    }

    private static Dictionary<string, AttributeValue> ToKeys(List<KeySchemaElement> schema, string[] keyValues)
    {
        return schema
            .Select((s, i) => (s.AttributeName, Value: new AttributeValue(keyValues[i])))
            .ToDictionary(t => t.AttributeName, t => t.Value);
    }

    public static IEnumerable<PSObject> Scan(this IAmazonDynamoDB dynamo, string tableName, int? limit)
    {
        var response = dynamo.ScanAsync(new ScanRequest
        {
            TableName = tableName,
            Limit = limit ?? 20
        }).GetAwaiter().GetResult();

        return response.Items.ToPSObjects();
    }

    private static IEnumerable<PSObject> ToPSObjects(this IEnumerable<Dictionary<string, AttributeValue>> items)
    {
        foreach (var item in items)
        {
            yield return item.ToPSObject();
        }
    }

    private static PSObject ToPSObject(this Dictionary<string, AttributeValue> item)
    {
        var psObject = new PSObject();
        foreach (var (name, value) in item)
        {
            psObject.SetProperty(name, value.Value());
        }

        return psObject;
    }

    private static object? Value(this AttributeValue value)
    {
        if (value.NULL)
        {
            return null;
        }
        
        if (value.S != null)
        {
            return value.S;
        }

        if (value.SS != null)
        {
            return value.SS.Select(v => v);
        }

        if (value.N != null)
        {
            return value.N.ToNumber();
        }
        
        if (value.NS != null)
        {
            return value.NS.Select(v => v.ToNumber());
        }

        if (value.B != null)
        {
            return value.B;
        }

        if (value.BS != null)
        {
            return value.BS;
        }

        if (value.IsBOOLSet)
        {
            return value.BOOL;
        }

        if (value.IsMSet)
        {
            return value.M.ToPSObject();
        }

        if (value.IsLSet)
        {
            return value.L.Select(v => v.Value());
        }

        return value;
    }

    private static object? ToNumber(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (value.Contains("."))
        {
            return Decimal.Parse(value);
        }

        return Int32.Parse(value);
    }
    
}