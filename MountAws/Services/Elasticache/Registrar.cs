using Amazon;
using Amazon.ElastiCache;
using Amazon.Runtime;
using Autofac;

namespace MountAws.Services.Elasticache;

public class Registrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AmazonElastiCacheClient>().As<IAmazonElastiCache>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}