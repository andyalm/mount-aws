using Amazon;
using Amazon.Route53;
using Amazon.Runtime;
using Autofac;
using MountAws.Api;

namespace MountAws.Services.Route53;

public class Route53Registrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AmazonRoute53Client>().As<IAmazonRoute53>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}