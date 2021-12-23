using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Provider;
using Amazon;
using Amazon.EC2;
using Amazon.ECS;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Autofac;
using MountAnything;
using MountAnything.Routing;
using MountAws.Api;
using MountAws.Services.EC2;
using MountAws.Services.ECR;
using MountAws.Services.ECS;
using MountAws.Services.ELBV2;
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
            builder.RegisterInstance<RegionEndpoint>(RegionEndpoint.USEast1);
            builder.RegisterInstance<AWSCredentials>(new AnonymousAWSCredentials());
            builder.RegisterType<AmazonEC2Client>().As<IAmazonEC2>()
                .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
            builder.RegisterType<AmazonECSClient>().As<IAmazonECS>()
                .UsingConstructor(typeof(AWSCredentials), typeof(RegionEndpoint));
        });
        router.MapRegex<ProfileHandler>("(?<Profile>[a-z0-9-_]+)", profile =>
        {
            var chain = new CredentialProfileStoreChain();
            profile.RegisterServices((match, builder) =>
            {
                var profileName = match.Values["Profile"];
                builder.RegisterInstance(new CurrentProfile(profileName));
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
                    var regionName = match.Values["Region"];
                    builder.RegisterInstance(new CurrentRegion(regionName));
                    builder.Register<RegionEndpoint>(c => RegionEndpoint.GetBySystemName(regionName));
                });
                region.MapLiteral<EC2Handler>("ec2", ec2 =>
                {
                    ec2.MapLiteral<EC2InstancesHandler>("instances", instances =>
                    {
                        instances.Map<EC2InstanceHandler>();
                    });
                });
                region.MapECR();
                region.MapLiteral<ECSRootHandler>("ecs", ecs =>
                {
                    ecs.MapLiteral<ClustersHandler>("clusters", clusters =>
                    {
                        clusters.Map<ClusterHandler>("CurrentCluster", cluster =>
                        {
                            cluster.RegisterServices((match, builder) =>
                            {
                                builder.RegisterInstance(new CurrentCluster(match.Values["CurrentCluster"]));
                            });
                            cluster.MapLiteral<ContainerInstancesHandler>("container-instances", containerInstances =>
                            {
                                containerInstances.Map<ContainerInstanceHandler>(containerInstance =>
                                {
                                    containerInstance.Map<TaskHandler>();
                                });
                            });
                            cluster.MapLiteral<ServicesHandler>("services", services =>
                            {
                                services.Map<ServiceHandler>(service =>
                                {
                                    service.Map<TaskHandler>();
                                });
                            });
                        });
                    });
                });
                region.MapLiteral<Elbv2RootHandler>("elbv2", elbv2 =>
                {
                    elbv2.MapLiteral<LoadBalancersHandler>("load-balancers", loadBalancers =>
                    {
                        loadBalancers.Map<LoadBalancerHandler>(loadBalancer =>
                        {
                            loadBalancer.Map<ListenerHandler>(listener =>
                            {
                                listener.MapLiteral<DefaultActionsHandler>("default-actions", defaultActions =>
                                {
                                    defaultActions.Map<DefaultActionHandler>(defaultAction =>
                                    {
                                        defaultAction.Map<TargetGroupHandler>();
                                    });
                                });
                                listener.MapLiteral<RulesHandler>("rules", rules =>
                                {
                                    rules.Map<RuleHandler>(rule =>
                                    {
                                        rule.Map<RuleActionHandler>(ruleAction =>
                                        {
                                            ruleAction.Map<TargetGroupHandler>(targetGroup =>
                                            {
                                                targetGroup.Map<TargetHealthHandler>();
                                            });
                                        });
                                    });
                                });
                            });
                        });
                    });
                });
                
                region.MapS3();
            });
        });

        return router;
    }
}
