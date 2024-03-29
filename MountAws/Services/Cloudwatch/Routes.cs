using Microsoft.Extensions.DependencyInjection;
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
                    logGroup.ConfigureServices((services, match) =>
                    {
                        services.AddSingleton(LogGroupName.Parse(match.Values["LogGroupName"]));
                    });
                    logGroup.MapLiteral<LogStreamsHandler>("streams", streams =>
                    {
                        streams.MapRegex<LogStreamHandler>(@"(?<LogStreamPath>[a-z0-9_+=,.@\-\$\[\]/]+)", stream =>
                        {
                            stream.ConfigureServices((services, match) =>
                            {
                                services.AddSingleton(LogStreamPath.Parse(match.Values["LogStreamPath"]));
                            });
                            stream.MapRegex<OutputLogEventHandler>(@"\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-5]\d\.\d+([+-][0-2]\d:[0-5]\d|Z)");
                        });
                    });
                });
            });
            cloudwatch.MapLiteral<MetricsHandler>("metrics", metrics =>
            {
                metrics.MapRegex<MetricHandler>(@"(?<MetricName>[a-z0-9_+=,.@\-/]+)", metric =>
                {
                    metric.ConfigureServices((services, match) =>
                    {
                        services.AddSingleton(MetricName.Parse(match.Values[nameof(MetricName)]));
                    });
                    metric.MapRegex<MetricTimeframeHandler>($"({string.Join("|",MetricTimeframe.All.Select(t => t.Name))})", timeframe =>
                    {
                        timeframe.MapRegex<MetricAggregationHandler>($"({string.Join("|", MetricAggregation.All.Select(a => a.Name))})");
                    });
                });
            });
        });
    }
}