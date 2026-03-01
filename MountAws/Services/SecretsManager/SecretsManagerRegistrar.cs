using Amazon;
using Amazon.SecretsManager;
using Amazon.Runtime;
using Autofac;

namespace MountAws.Services.SecretsManager;

public class SecretsManagerRegistrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AmazonSecretsManagerClient>().As<IAmazonSecretsManager>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}
