using MountAnything;

namespace MountAws.Services.DynamoDb;

public class CurrentTable(string tableName) : TypedString(tableName)
{
    public string Name => Value;
}