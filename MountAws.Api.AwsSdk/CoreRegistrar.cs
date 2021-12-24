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
                TryGetAWSCredentials(currentProfile, out var credentials))
            {
                return credentials;
            }

            return new AnonymousAWSCredentials();
        }).As<AWSCredentials>();
        builder.Register<RegionEndpoint>(c => RegionEndpoint.GetBySystemName(c.Resolve<CurrentRegion>()));
        builder.RegisterType<AwsSdkCoreApi>().As<ICoreApi>();
    }

    private bool TryGetAWSCredentials(string profileName, out AWSCredentials credentials, HashSet<string>? profileBreadCrumbs = null)
    {
        profileBreadCrumbs ??= new HashSet<string>();
        if (profileBreadCrumbs.Contains(profileName))
        {
            throw new StackOverflowException("Your aws profiles have an infinite loop");
        }
        profileBreadCrumbs.Add(profileName);
        
        var profileChain = new CredentialProfileStoreChain();
        if (!profileChain.TryGetProfile(profileName, out var profile))
        {
            credentials = default!;
            return false;
        }

        if (!string.IsNullOrWhiteSpace(profile.Options.SourceProfile))
        {
            if (!TryGetAWSCredentials(profile.Options.SourceProfile, out var sourceCredentials))
            {
                credentials = default!;
                return false;
            }

            credentials = new SourceProfileAWSCredentials(sourceCredentials, profile.Options.RoleArn,
                profile.Options.RoleSessionName, new AssumeRoleAWSCredentialsOptions
                {
                    ExternalId = profile.Options.ExternalID,
                    MfaSerialNumber = profile.Options.MfaSerial
                });
            return true;
        }

        return profileChain.TryGetAWSCredentials(profileName, out credentials);
    }
}