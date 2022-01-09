using Amazon;
using Amazon.AutoScaling;
using Amazon.EC2;
using Amazon.Runtime;
using Autofac;

namespace MountAws.Services.Ec2;

public class Ec2Registrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AmazonEC2Client>().As<IAmazonEC2>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
        builder.RegisterType<AmazonAutoScalingClient>().As<IAmazonAutoScaling>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}