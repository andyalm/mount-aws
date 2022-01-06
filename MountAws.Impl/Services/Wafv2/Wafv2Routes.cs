using Amazon.WAFV2;
using Autofac;
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
            webAcl.MapLiteral<DefaultActionHandler>("default-action", defaultAction =>
            {
                defaultAction.MapLiteral<CustomHeadersHandler>("custom-request-headers");
                defaultAction.MapLiteral<CustomHeadersHandler>("custom-response-headers");
            });
        });
    }
}