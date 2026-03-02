using Autofac;
using MountAnything;
using MountAnything.Routing;

namespace MountAws.Services.AppAutoscaling;

public static class RouteExtensions
{
    public static void MapAppAutoscaling<TItem>(this Route route, string serviceNamespace, Func<TItem,string> resourceIdSelector) where TItem : IItem
    {
        route.ConfigureContainer(c =>
        {
            c.RegisterInstance(new CurrentServiceNamespace(serviceNamespace));
            c.Register<IResourceIdResolver>(s =>
            {
                var item = s.Resolve<IItemAncestor<TItem>>();

                return new ItemResourceIdResolver<TItem>(item, resourceIdSelector);
            }).As<IResourceIdResolver>();
        });
        route.MapLiteral<AutoscalingHandler>("autoscaling", autoscaling =>
        {
            autoscaling.MapLiteral<ScalingPoliciesHandler>("scaling-policies", scalablePolicies =>
            {
                scalablePolicies.Map<ScalingPolicyHandler>();
            });
            autoscaling.MapLiteral<ScalingActivitiesHandler>("scaling-activities", scalingActivities =>
            {
                scalingActivities.Map<ScalingActivityHandler>();
            });
            autoscaling.MapLiteral<ScheduledActionsHandler>("scheduled-actions", scheduledActions =>
            {
                scheduledActions.Map<ScheduledActionHandler>();
            });
        });
    }
}