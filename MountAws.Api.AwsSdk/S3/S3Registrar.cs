using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Autofac;
using MountAws.Api.S3;

namespace MountAws.Api.AwsSdk.S3;

public class S3Registrar : IApiServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AwsSdkS3Api>().As<IS3Api>();
        builder.RegisterType<AmazonS3Client>().As<IAmazonS3>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}