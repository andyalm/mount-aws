using Amazon;
using Amazon.ECS;
using Amazon.Runtime;
using Autofac;
using MountAws.Api.Ecs;

namespace MountAws.Api.AwsSdk.Ecs;

public class EcsRegistrar : IApiServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AwsSdkEcsApi>().As<IEcsApi>();
        builder.RegisterType<AmazonECSClient>().As<IAmazonECS>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}