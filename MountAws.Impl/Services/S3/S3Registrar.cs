using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Autofac;
using MountAws.Api;

namespace MountAws.Services.S3;

public class S3Registrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AmazonS3Client>().As<IAmazonS3>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}