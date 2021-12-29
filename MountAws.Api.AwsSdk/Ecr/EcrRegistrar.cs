using Amazon;
using Amazon.ECR;
using Amazon.Runtime;
using Autofac;
using MountAws.Api.Ecr;

namespace MountAws.Api.AwsSdk.ECR;

public class EcrRegistrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AwsSdkEcrApi>().As<IEcrApi>();
        builder.RegisterType<AmazonECRClient>().As<IAmazonECR>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}