using Amazon;
using Amazon.ECR;
using Amazon.Runtime;
using Autofac;
using MountAws.Api;

namespace MountAws.Services.Ecr;

public class EcrRegistrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AmazonECRClient>().As<IAmazonECR>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}