using Amazon;
using Amazon.EC2;
using Amazon.Runtime;
using Autofac;
using MountAws.Api.Ec2;

namespace MountAws.Api.AwsSdk.Ec2;

public class Ec2Registrar : IApiServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AwsSdkEc2Api>().As<IEc2Api>();
        builder.RegisterType<AmazonEC2Client>().As<IAmazonEC2>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}