using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Autofac;

namespace MountAws.Api.AwsSdk;

public class CoreRegistrar : IApiServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.Register(c =>
        {
            if (c.TryResolve<CurrentProfile>(out var currentProfile) &&
                new CredentialProfileStoreChain().TryGetAWSCredentials(currentProfile, out var credentials))
            {
                return credentials;
            }

            return new AnonymousAWSCredentials();
        }).As<AWSCredentials>();
        builder.Register<RegionEndpoint>(c => RegionEndpoint.GetBySystemName(c.Resolve<CurrentRegion>()));

    }
}