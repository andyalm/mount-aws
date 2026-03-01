using MountAnything.Routing;

namespace MountAws.Services.Autoscaling;

public class Routes : IServiceRoutes
{
    public void AddServiceRoutes(Route regionRoute)
    {
        regionRoute.MapLiteral<RootHandler>("autoscaling", autoscaling =>
        {
            autoscaling.MapLiteral<ServicesHandler>("services", services =>
            {
                services.Map<ServiceHandler, CurrentServiceNamespace>(service =>
                {
                    service.MapLiteral<ScalableTargetsHandler>("scalable-targets", scalableTargets =>
                    {
                        scalableTargets.Map<ScalableTargetHandler, CurrentResourceId>(scalableTarget =>
                        {
                            scalableTarget.MapLiteral<ScalingPoliciesHandler>("scaling-policies", scalingPolicies =>
                            {
                                scalingPolicies.Map<ScalingPolicyHandler>();
                            });
                            scalableTarget.MapLiteral<ScalingActivitiesHandler>("scaling-activities");
                            scalableTarget.MapLiteral<ScheduledActionsHandler>("scheduled-actions", scheduledActions =>
                            {
                                scheduledActions.Map<ScheduledActionHandler>();
                            });
                        });
                    });
                });
            });
        });
    }
}
