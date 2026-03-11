using Amazon;
using Amazon.ApplicationAutoScaling;
using Amazon.Runtime;
using Autofac;

namespace MountAws.Services.AppAutoscaling;

public class Registrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AmazonApplicationAutoScalingClient>()
            .As<IAmazonApplicationAutoScaling>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}
