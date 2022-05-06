using Amazon;
using Amazon.IdentityManagement;
using Amazon.Runtime;
using Autofac;

namespace MountAws.Services.Iam;

public class Registrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AmazonIdentityManagementServiceClient>().As<IAmazonIdentityManagementService>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}