using Amazon;
using Amazon.Runtime;
using Amazon.ServiceDiscovery;
using Autofac;

namespace MountAws.Services.ServiceDiscovery;

public class ServiceDiscoveryRegistrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AmazonServiceDiscoveryClient>().As<IAmazonServiceDiscovery>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}