using Autofac;
using Microsoft.Extensions.DependencyInjection;
using MountAnything.Routing;

namespace MountAws.Services.S3;

public class Routes : IServiceRoutes
{
    public void AddServiceRoutes(Route route)
    {
        route.MapLiteral<S3RootHandler>("s3", s3 =>
        {
            s3.MapLiteral<BucketsHandler>("buckets", buckets =>
            {
                buckets.Map<BucketHandler, CurrentBucket>(bucket =>
                {
                    bucket.MapLiteral<PolicyHandler>("policy");
                    bucket.MapLiteral<ObjectsHandler>("objects", objects =>
                    {
                        objects.MapRegex<ObjectHandler>(@"(?<ObjectPath>.+)", @object =>
                        {
                            @object.ConfigureServices((services, match) =>
                            {
                                services.AddSingleton(new ObjectPath(match.Values["ObjectPath"]));
                            });
                        });
                    });
                });
            });
        });
    }
}