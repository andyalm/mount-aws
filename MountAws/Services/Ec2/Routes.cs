using MountAnything.Routing;

namespace MountAws.Services.Ec2;

public static class Routes
{
    public static void MapEc2(this Route route)
    {
        route.MapLiteral<Ec2RootHandler>("ec2", ec2 =>
        {
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