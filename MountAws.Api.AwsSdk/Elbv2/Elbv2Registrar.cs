using Amazon;
using Amazon.ElasticLoadBalancingV2;
using Amazon.Runtime;
using Autofac;
using MountAws.Api.Elbv2;

namespace MountAws.Api.AwsSdk.Elbv2;

public class Elbv2Registrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AwsSdkElbv2Api>().As<IElbv2Api>();
        builder.RegisterType<AmazonElasticLoadBalancingV2Client>().As<IAmazonElasticLoadBalancingV2>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}