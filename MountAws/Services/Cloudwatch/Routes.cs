using Autofac;
using MountAnything.Routing;

namespace MountAws.Services.Cloudwatch;

public class Routes : IServiceRoutes
{
    public void AddServiceRoutes(Route regionRoute)
    {
        regionRoute.MapLiteral<RootHandler>("cloudwatch", cloudwatch =>
        {
            cloudwatch.MapLiteral<LogGroupsHandler>("log-groups", logGroups =>
            {
                logGroups.MapRegex<LogGroupHandler>(@"(?<LogGroupName>[a-z0-9_+=,.@\-/]+)", logGroup =>
                {
                    logGroup.RegisterServices((match, builder) =>
                    {
                        builder.RegisterInstance(LogGroupName.Parse(match.Values["LogGroupName"]));
                    });
                });
            });
        });
    }
}