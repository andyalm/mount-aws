using MountAnything.Routing;

namespace MountAws.Services.Elbv2;

public static class Routes
{
    public static void MapElbv2(this Route route)
    {
        route.MapLiteral<Elbv2RootHandler>("elbv2", elbv2 =>
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
                                defaultAction.MapTargetGroup();
                            });
                        });
                        listener.MapLiteral<RulesHandler>("rules", rules =>
                        {
                            rules.Map<RuleHandler>(rule =>
                            {
                                rule.Map<RuleActionHandler>(ruleAction =>
                                {
                                    ruleAction.MapTargetGroup();
                                });
                            });
                        });
                    });
                });
            });
            elbv2.MapLiteral<TargetGroupsHandler>("target-groups", targetGroups =>
            {
                targetGroups.MapTargetGroup();
            });
        });
    }

    private static void MapTargetGroup(this Route targetGroupRoute)
    {
        targetGroupRoute.Map<TargetGroupHandler>(targetGroup =>
        {
            targetGroup.Map<TargetHealthHandler>();
        });
    }
}