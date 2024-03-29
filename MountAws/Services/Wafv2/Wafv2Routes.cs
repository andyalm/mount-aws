using Amazon.WAFV2;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using MountAnything;
using MountAnything.Routing;

namespace MountAws.Services.Wafv2;

public class Wafv2Routes : IServiceRoutes
{
    public void AddServiceRoutes(Route regionRoute)
    {
        regionRoute.MapLiteral<Wafv2RootHandler>("wafv2", wafv2 =>
        {
            wafv2.MapLiteral<WebAclsHandler>("cloudfront-web-acls", cloudfrontWebAcls =>
            {
                cloudfrontWebAcls.ConfigureServices((services) =>
                {
                    services.AddTransient(_ => Scope.CLOUDFRONT);
                });
                
                cloudfrontWebAcls.MapWebAcl();
            });
            wafv2.MapLiteral<WebAclsHandler>("regional-web-acls", regionalWebAcls =>
            {
                regionalWebAcls.ConfigureServices((services) =>
                {
                    services.AddTransient(_ => Scope.REGIONAL);
                });
                
                regionalWebAcls.MapWebAcl();
            });
        });
    }
}

public static class Wafv2RouteExtensions
{
    public static void MapWebAcl(this IRoutable route)
    {
        route.Map<WebAclHandler>(webAcl =>
        {
            webAcl.ConfigureServices(services =>
            {
                services.AddScoped(c =>
                {
                    var webAclItem = c.GetRequiredService<IItemAncestor<WebAclItem>>().Item;
                    var wafv2 = c.GetRequiredService<IAmazonWAFV2>();
                    var scope = c.GetRequiredService<Scope>();

                    return wafv2.GetWebAcl(scope, (webAclItem.Id, webAclItem.ItemName));
                });
            });
            webAcl.MapAction<DefaultActionHandler>("default-action");
            webAcl.MapLiteral<RulesHandler>("rules", rules =>
            {
                rules.Map<RuleHandler>(rule =>
                {
                    rule.MapAction<RuleActionHandler>("action");
                    rule.MapLiteral<StatementHandler>("statement", statement =>
                    {
                        statement.ConfigureServices((services, match) =>
                        {
                            if (match.Values.TryGetValue("StatementPath", out var statementPath))
                            {
                                services.AddSingleton(new StatementPath(statementPath));
                            }
                            else
                            {
                                services.AddSingleton(new StatementPath(ItemPath.Root));
                            }
                        });
                        statement.MapRegex<StatementHandler>("(?<StatementPath>[a-z0-9-_/]+)");
                    });
                });
            });
        });
    }

    public static void MapAction<TActionHandler>(this IRoutable route, string actionName) where TActionHandler : ActionPathHandler
    {
        route.MapLiteral<TActionHandler>(actionName, action =>
        {
            action.MapLiteral<CustomHeadersHandler>("custom-request-headers");
            action.MapLiteral<CustomHeadersHandler>("custom-response-headers");
        });
    }
}