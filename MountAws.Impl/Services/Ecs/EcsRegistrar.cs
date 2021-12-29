using Amazon;
using Amazon.ECS;
using Amazon.Runtime;
using Autofac;
using MountAws.Api;

namespace MountAws.Services.Ecs;

public class EcsRegistrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AmazonECSClient>().As<IAmazonECS>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}