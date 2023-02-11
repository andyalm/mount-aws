using Amazon;
using Amazon.RDS;
using Amazon.Runtime;
using Autofac;

namespace MountAws.Services.Rds;

public class Registrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AmazonRDSClient>().As<IAmazonRDS>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}