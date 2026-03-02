using MountAnything.Routing;
using MountAws.Services.AppAutoscaling;

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
                    table.MapAppAutoscaling<TableItem>("dynamodb", item => $"table/{item.ItemName}");
                    table.MapLiteral<TableItemsHandler>("items", items =>
                    {
                        items.MapRegex<ItemHandler>(@"[a-z0-9-_\.\,]+");
                    });
                });
            });
        });
    }
}