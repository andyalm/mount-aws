using Amazon;
using Amazon.CloudWatch;
using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using Autofac;

namespace MountAws.Services.Cloudwatch;

public class ServiceRegistrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AmazonCloudWatchClient>().As<IAmazonCloudWatch>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
        builder.RegisterType<AmazonCloudWatchLogsClient>().As<IAmazonCloudWatchLogs>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}