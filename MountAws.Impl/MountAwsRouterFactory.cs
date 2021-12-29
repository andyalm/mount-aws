using Autofac;
using MountAnything.Routing;
using MountAws.Api;
using MountAws.Api.AwsSdk;
using MountAws.Api.AwsSdk.Ec2;
using MountAws.Host.Abstractions;
using MountAws.Services.Core;
using MountAws.Services.Ec2;
using MountAws.Services.Ecr;
using MountAws.Services.Ecs;
using MountAws.Services.Elbv2;
using MountAws.Services.Route53;
using MountAws.Services.S3;

namespace MountAws;

public class MountAwsRouterFactory : IRouterFactory
{
    public Router CreateRouter()
    {
        var router = Router.Create<ProfilesHandler>();
        router.RegisterServices(builder =>
        {
            var registrars = typeof(CoreServiceRegistrar).Assembly
                .GetTypes()
                .Where(t => typeof(IServiceRegistrar).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IServiceRegistrar>();
            foreach (var registrar in registrars)
            {
                registrar.Register(builder);
            }
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
                region.MapEc2();
                region.MapEcr();
                region.MapEcs();
                region.MapElbv2();
                region.MapRoute53();
                region.MapS3();
            });
        });

        return router;
    }
}