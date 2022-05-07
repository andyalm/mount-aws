using MountAnything.Routing;

namespace MountAws.Services.DynamoDb;

public class Routes : IServiceRoutes
{
    public void AddServiceRoutes(Route regionRoute)
    {
        regionRoute.MapLiteral<DynamoDbRootHandler>("dynamodb", dynamodb =>
        {
            dynamodb.MapLiteral<TablesHandler>("tables", tables =>
            {
                tables.Map<TableHandler>(table =>
                {
                    table.MapRegex<ItemHandler>(@"[a-z0-9-_\.\,]+");
                });
            });
        });
    }
}