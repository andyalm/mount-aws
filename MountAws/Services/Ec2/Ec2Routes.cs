using MountAnything.Routing;

namespace MountAws.Services.Ec2;

public class Ec2Routes : IServiceRoutes
{
    public void AddServiceRoutes(Route route)
    {
        route.MapLiteral<Ec2RootHandler>("ec2", ec2 =>
        {
            ec2.MapLiteral<AutoScalingGroupsHandler>("auto-scaling-groups", autoScalingGroups =>
            {
                autoScalingGroups.Map<AutoScalingGroupHandler>(autoScalingGroup =>
                {
                    autoScalingGroup.Map<InstanceHandler>();
                });
            });
            ec2.MapLiteral<ImagesHandler>("images", images =>
            {
                images.Map<ImageHandler>();
            });
            ec2.MapLiteral<InstancesHandler>("instances", instances =>
            {
                instances.Map<InstanceHandler>();
            });
            ec2.MapLiteral<SecurityGroupsHandler>("security-groups", securityGroups =>
            {
                securityGroups.Map<SecurityGroupHandler>();
            });
            ec2.MapLiteral<VpcsHandler>("vpcs", vpcs =>
            {
                vpcs.Map<VpcHandler>(vpc =>
                {
                    vpc.Map<SubnetHandler>();
                });
            });
        });
    }
}