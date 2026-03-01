using System;
using AwesomeAssertions;
using MountAnything;
using MountAnything.Routing;
using MountAws.Services.Autoscaling;
using Xunit;

namespace MountAws.UnitTests;

public class AutoscalingTests
{
    private readonly Router _router;

    public AutoscalingTests()
    {
        var provider = new MountAwsProvider();
        _router = provider.CreateRouter();
    }

    [Theory]
    [InlineData("myprofile/us-east-1/autoscaling", typeof(RootHandler))]
    [InlineData("myprofile/us-east-1/autoscaling/services", typeof(ServicesHandler))]
    [InlineData("myprofile/us-east-1/autoscaling/services/dynamodb", typeof(ServiceHandler))]
    [InlineData("myprofile/us-east-1/autoscaling/services/ecs", typeof(ServiceHandler))]
    [InlineData("myprofile/us-east-1/autoscaling/services/dynamodb/scalable-targets", typeof(ScalableTargetsHandler))]
    [InlineData("myprofile/us-east-1/autoscaling/services/dynamodb/scalable-targets/table:my-table", typeof(ScalableTargetHandler))]
    [InlineData("myprofile/us-east-1/autoscaling/services/dynamodb/scalable-targets/table:my-table/scaling-policies", typeof(ScalingPoliciesHandler))]
    [InlineData("myprofile/us-east-1/autoscaling/services/dynamodb/scalable-targets/table:my-table/scaling-policies/my-policy", typeof(ScalingPolicyHandler))]
    [InlineData("myprofile/us-east-1/autoscaling/services/dynamodb/scalable-targets/table:my-table/scaling-activities", typeof(ScalingActivitiesHandler))]
    [InlineData("myprofile/us-east-1/autoscaling/services/dynamodb/scalable-targets/table:my-table/scheduled-actions", typeof(ScheduledActionsHandler))]
    [InlineData("myprofile/us-east-1/autoscaling/services/dynamodb/scalable-targets/table:my-table/scheduled-actions/my-action", typeof(ScheduledActionHandler))]
    public void AutoscalingRoutesResolveToCorrectHandlers(string path, Type expectedHandlerType)
    {
        var resolver = _router.GetResolver(new ItemPath(path));
        resolver.HandlerType.Should().Be(expectedHandlerType);
    }
}
