using Amazon;
using Amazon.Runtime;
using Amazon.WAFV2;
using Autofac;

namespace MountAws.Services.Wafv2;

public class Wafv2Registrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AmazonWAFV2Client>().As<IAmazonWAFV2>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}