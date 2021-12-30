using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Autofac;
using MountAws.Api;

namespace MountAws.Services.Core;

public class CoreServiceRegistrar : IServiceRegistrar
{
    public void Register(ContainerBuilder builder)
    {
        builder.RegisterInstance(new CurrentRegion("us-east-1"));
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
    }
    
    private static bool TryGetAWSCredentials(string profileName, out AWSCredentials credentials, HashSet<string>? profileBreadCrumbs = null)
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
                profile.Options.RoleSessionName ?? "MountAws", new AssumeRoleAWSCredentialsOptions
                {
                    ExternalId = profile.Options.ExternalID,
                    MfaSerialNumber = profile.Options.MfaSerial
                });
            return true;
        }

        return profileChain.TryGetAWSCredentials(profileName, out credentials);
    }
}