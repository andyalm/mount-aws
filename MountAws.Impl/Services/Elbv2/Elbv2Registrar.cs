using Amazon;
using Amazon.ElasticLoadBalancingV2;
using Amazon.Runtime;
using Autofac;

namespace MountAws.Services.Elbv2;

public class Elbv2Registrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AmazonElasticLoadBalancingV2Client>().As<IAmazonElasticLoadBalancingV2>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}