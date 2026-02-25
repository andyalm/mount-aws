using MountAnything.Routing;

namespace MountAws.Services.Lambda;

public class LambdaRoutes : IServiceRoutes
{
    public void AddServiceRoutes(Route route)
    {
        route.MapLiteral<LambdaRootHandler>("lambda", lambda =>
        {
            lambda.MapLiteral<FunctionsHandler>("functions", functions =>
            {
                functions.Map<FunctionHandler, CurrentFunction>(function =>
                {
                    function.MapLiteral<AliasesHandler>("aliases", aliases =>
                    {
                        aliases.Map<AliasHandler>();
                    });
                    function.MapLiteral<VersionsHandler>("versions", versions =>
                    {
                        versions.Map<VersionHandler>();
                    });
                    function.MapLiteral<EventSourceMappingsHandler>("event-source-mappings",
                        eventSourceMappings =>
                    {
                        eventSourceMappings.Map<EventSourceMappingHandler>();
                    });
                });
            });
            lambda.MapLiteral<LayersHandler>("layers", layers =>
            {
                layers.Map<LayerHandler, CurrentLayer>(layer =>
                {
                    layer.Map<LayerVersionHandler>();
                });
            });
        });
    }
}
