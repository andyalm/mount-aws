using Autofac;
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
                buckets.Map<BucketHandler>("S3Bucket", bucket =>
                {
                    bucket.RegisterServices((match, builder) =>
                    {
                        builder.RegisterInstance(new CurrentBucket(match.Values["S3Bucket"]));
                    });
                    bucket.MapLiteral<PolicyHandler>("policy");
                    bucket.MapLiteral<ObjectsHandler>("objects", objects =>
                    {
                        objects.MapRegex<ObjectHandler>(@"(?<ObjectPath>.+)", @object =>
                        {
                            @object.RegisterServices((match, builder) =>
                            {
                                builder.RegisterInstance(new ObjectPath(match.Values["ObjectPath"]));
                            });
                        });
                    });
                });
            });
        });
    }
}