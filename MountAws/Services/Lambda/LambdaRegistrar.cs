using Amazon;
using Amazon.Lambda;
using Amazon.Runtime;
using Autofac;

namespace MountAws.Services.Lambda;

public class LambdaRegistrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AmazonLambdaClient>().As<IAmazonLambda>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}
