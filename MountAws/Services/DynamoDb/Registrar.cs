using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Autofac;

namespace MountAws.Services.DynamoDb;

public class Registrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterType<AmazonDynamoDBClient>().As<IAmazonDynamoDB>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
    }
}