using Amazon.WAFV2;
using Autofac;
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
                cloudfrontWebAcls.RegisterServices((match, builder) =>
                {
                    builder.Register(_ => Scope.CLOUDFRONT);
                });
                
                cloudfrontWebAcls.MapWebAcl();
            });
            wafv2.MapLiteral<WebAclsHandler>("regional-web-acls", regionalWebAcls =>
            {
                regionalWebAcls.RegisterServices((match, builder) =>
                {
                    builder.Register(_ => Scope.REGIONAL);
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
            webAcl.RegisterServices((match, builder) =>
            {
                builder.Register(c =>
                {
                    var webAclItem = c.Resolve<IItemAncestor<WebAclItem>>().Item;
                    var wafv2 = c.Resolve<IAmazonWAFV2>();
                    var scope = c.Resolve<Scope>();

                    return wafv2.GetWebAcl(scope, (webAclItem.Id, webAclItem.ItemName));
                }).InstancePerLifetimeScope();
            });
            webAcl.MapAction<DefaultActionHandler>("default-action");
            webAcl.MapLiteral<RulesHandler>("rules", rules =>
            {
                rules.Map<RuleHandler>(rule =>
                {
                    rule.MapAction<RuleActionHandler>("action");
                    rule.MapLiteral<StatementHandler>("statement", statement =>
                    {
                        statement.RegisterServices((match, builder) =>
                        {
                            if (match.Values.TryGetValue("StatementPath", out var statementPath))
                            {
                                builder.RegisterInstance(new StatementPath(statementPath));
                            }
                            else
                            {
                                builder.RegisterInstance(new StatementPath(ItemPath.Root));
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