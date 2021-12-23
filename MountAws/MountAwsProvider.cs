using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Provider;
using Autofac;
using MountAnything;
using MountAnything.Routing;
using MountAws.Api;
using MountAws.Services.Ec2;
using MountAws.Services.Ecr;
using MountAws.Services.Ecs;
using MountAws.Services.Elbv2;
using MountAws.Services.S3;

namespace MountAws;
[CmdletProvider("MountAws", ProviderCapabilities.ExpandWildcards | ProviderCapabilities.Filter)]
public class MountAwsProvider : MountAnythingProvider
{
    protected override Collection<PSDriveInfo> InitializeDefaultDrives()
    {
        return new Collection<PSDriveInfo>
        {
            new("aws", ProviderInfo, ItemSeparator.ToString(),
                "Allows you to navigate aws services as a virtual filesystem", null)
        };
    }

    public override Router CreateRouter()
    {
        var router = Router.Create<ProfilesHandler>();
        router.RegisterServices(builder =>
        {
            builder.RegisterInstance(new CurrentRegion("us-east-1"));
        });
        router.MapRegex<ProfileHandler>("(?<Profile>[a-z0-9-_]+)", profile =>
        {
            profile.RegisterServices((match, builder) =>
            {
                builder.RegisterInstance(new CurrentProfile(match.Values["Profile"]));
            });
            profile.MapRegex<RegionHandler>("(?<Region>[a-z0-9-]+)", region =>
            {
                region.RegisterServices((match, builder) =>
                {
                    var regionName = match.Values["Region"];
                    builder.RegisterInstance(new CurrentRegion(regionName));
                });
                region.MapLiteral<Ec2RootHandler>("ec2", ec2 =>
                {
                    ec2.MapLiteral<InstancesHandler>("instances", instances =>
                    {
                        instances.Map<InstanceHandler>();
                    });
                });
                region.MapEcr();
                region.MapEcs();
                region.MapElbv2();
                region.MapS3();
            });
        });

        return router;
    }
}
