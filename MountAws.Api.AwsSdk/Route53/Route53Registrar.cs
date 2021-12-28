using Amazon;
using Amazon.Route53;
using Amazon.Runtime;
using Autofac;
using MountAws.Api.Route53;

namespace MountAws.Api.AwsSdk.Route53;

public class Route53Registrar : IApiServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AwsSdkRoute53Api>().As<IRoute53Api>();
        builder.RegisterType<AmazonRoute53Client>().As<IAmazonRoute53>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}