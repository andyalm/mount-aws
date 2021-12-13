using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Provider;
using Amazon;
using Amazon.EC2;
using Amazon.ElasticLoadBalancingV2;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Autofac;
using Autofac.Features.ResolveAnything;
using MountAnything;
using MountAnything.Routing;
using MountAws.Services.EC2;
using MountAws.Services.ELBV2;

namespace MountAws;
[CmdletProvider("MountAws", ProviderCapabilities.ExpandWildcards | ProviderCapabilities.Filter)]
public class MountAwsProvider : MountAnythingProvider
{
    private static readonly ILifetimeScope _container;
    private static readonly Router _router;
    
    public override ILifetimeScope Container => _container;
    public override Router Router => _router;

    protected override Collection<PSDriveInfo> InitializeDefaultDrives()
    {
        return new Collection<PSDriveInfo>
        {
            new("aws", ProviderInfo, ItemSeparator.ToString(),
                "Allows you to navigate aws services as a virtual filesystem", null)
        };
    }

    static MountAwsProvider()
    {
        _router = new Router()
            .MapRoot<ProfilesHandler>()
            .MapRegex<ProfileHandler>("(?<Profile>[a-z0-9-_]+)", profile =>
        {
            var chain = new CredentialProfileStoreChain();
            profile.RegisterServices((match, builder) =>
            {
                var profileName = match.Groups["Profile"].Value;
                if (chain.TryGetProfile(profileName, out var awsProfile))
                {
                    builder.RegisterInstance(awsProfile);
                }
                if (chain.TryGetAWSCredentials(profileName, out var awsCredentials))
                {
                    builder.RegisterInstance(awsCredentials);
                }
            });
            profile.MapRegex<RegionHandler>("(?<Region>[a-z0-9-]+)", region =>
            {
                region.RegisterServices((match, builder) =>
                {
                    var regionName = match.Groups["Region"].Value;
                    builder.Register<RegionEndpoint>(c => RegionEndpoint.GetBySystemName(regionName));
                });
                region.MapRegex<EC2Handler>("ec2", ec2 =>
                {
                    ec2.MapRegex<EC2InstancesHandler>("instances", instances =>
                    {
                        instances.MapRegex<EC2InstanceHandler>(@"(?<InstanceItemName>[a-z0-9-\.]+)");
                    });
                });
                region.MapRegex<ELBV2Handler>("elbv2", elbv2 =>
                {
                    elbv2.MapRegex<LoadBalancersHandler>("load-balancers", loadBalancers =>
                    {
                        loadBalancers.MapRegex<LoadBalancerHandler>(@"(?<LoadBalancerName>[a-z0-9-]+)", loadBalancer =>
                        {
                            loadBalancer.MapRegex<ListenerHandler>(@"(?<ListenerPort>[a-z0-9]+)", listener =>
                            {
                                listener.MapRegex<DefaultActionsHandler>("default-actions", defaultActions =>
                                {
                                    defaultActions.MapRegex<DefaultActionHandler>("(?<DefaultActionName>[a-z0-9-]+)",
                                        defaultAction =>
                                        {
                                            defaultAction.MapRegex<TargetGroupHandler>("(?<TargetGroupName>[a-z0-9-]+)");
                                        });
                                });
                                listener.MapRegex<RulesHandler>("rules", rules =>
                                {
                                    rules.MapRegex<RuleHandler>("(?<RulePriority>[a-z0-9]+)", rule =>
                                    {
                                        rule.MapRegex<RuleActionHandler>("(?<RuleActionName>[a-z0-9-]+)", ruleAction =>
                                        {
                                            ruleAction.MapRegex<TargetGroupHandler>("(?<TargetGroupName>[a-z0-9-]+)",
                                                targetGroup =>
                                                {
                                                    targetGroup.MapRegex<TargetHealthHandler>(@"(?<TargetHealthId>[a-z0-9-_:|]+)");
                                                });
                                        });
                                    });
                                });
                            });
                        });
                    });
                });
            });
        });
        
        var containerBuilder = new ContainerBuilder();
        containerBuilder.RegisterInstance(_router);
        containerBuilder.RegisterInstance<RegionEndpoint>(RegionEndpoint.USEast1);
        containerBuilder.RegisterInstance<AWSCredentials>(new AnonymousAWSCredentials());
        containerBuilder.RegisterType<AmazonEC2Client>().As<IAmazonEC2>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
        containerBuilder.RegisterType<AmazonElasticLoadBalancingV2Client>().As<IAmazonElasticLoadBalancingV2>()
            .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
        containerBuilder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

        _container = containerBuilder.Build();
    }
}
