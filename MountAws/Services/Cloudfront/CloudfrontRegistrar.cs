using Amazon;
using Amazon.CloudFront;
using Amazon.Runtime;
using Autofac;

namespace MountAws.Services.Cloudfront;

public class CloudfrontRegistrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AmazonCloudFrontClient>().As<IAmazonCloudFront>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}