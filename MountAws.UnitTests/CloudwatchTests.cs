using System;
using Autofac;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MountAnything;
using MountAnything.Routing;
using MountAws.Services.Cloudwatch;
using Xunit;

namespace MountAws.UnitTests;

public class CloudwatchTests
{
    private readonly Router _router;
    
    public CloudwatchTests()
    {
        var provider = new MountAwsProvider();
        _router = provider.CreateRouter();
    }
    
    [Theory]
    [InlineData("myprofile/us-east-1/cloudwatch", typeof(RootHandler))]
    [InlineData("myprofile/us-east-1/cloudwatch/log-groups", typeof(LogGroupsHandler))]
    [InlineData("myprofile/us-east-1/cloudwatch/log-groups/Gitlab-ECS-shared", typeof(LogGroupHandler))]
    [InlineData("myprofile/us-east-1/cloudwatch/log-groups/Gitlab-ECS-shared/aws", typeof(LogGroupHandler))]
    [InlineData("myprofile/us-east-1/cloudwatch/log-groups/Gitlab-ECS-shared/aws/lambda", typeof(LogGroupHandler))]
    [InlineData("myprofile/us-east-1/cloudwatch/log-groups/Gitlab-ECS-shared/aws/lambda/streams", typeof(LogStreamsHandler))]
    [InlineData("myprofile/us-east-1/cloudwatch/log-groups/Gitlab-ECS-shared/streams", typeof(LogStreamsHandler))]
    [InlineData("myprofile/us-east-1/cloudwatch/log-groups/Gitlab-ECS-shared/streams/gitlab-container/Gitlab-Web-shared", typeof(LogStreamHandler))]
    [InlineData("myprofile/us-east-1/cloudwatch/log-groups/Gitlab-ECS-shared/streams/gitlab-container/Gitlab-Web-shared/08725031925e46e89cf5f1f7ad4aa9e4", typeof(LogStreamHandler))]
    [InlineData("myprofile/us-east-1/cloudwatch/log-groups/Gitlab-ECS-shared/streams/gitlab-container/Gitlab-Web-shared/08725031925e46e89cf5f1f7ad4aa9e4/2022-06-06T15:11:24.8716500Z", typeof(OutputLogEventHandler))]
    public void NestedStreamsRouteMatchesLogStreamHandler(string path, Type expectedHandlerType)
    {
        var resolver = _router.GetResolver(new ItemPath(path));
        resolver.HandlerType.Should().Be(expectedHandlerType);
    }
    
    [Theory]
    [InlineData("myprofile/us-east-1/cloudwatch/metrics", typeof(MetricsHandler))]
    [InlineData("myprofile/us-east-1/cloudwatch/metrics/AWS", typeof(MetricHandler), "AWS")]
    [InlineData("myprofile/us-east-1/cloudwatch/metrics/AWS/ELB", typeof(MetricHandler), "AWS/ELB")]
    [InlineData("myprofile/us-east-1/cloudwatch/metrics/AWS/ELB/HTTPCode_Backend_4XX", typeof(MetricHandler), "AWS/ELB/HTTPCode_Backend_4XX")]
    public void NestedMetricsRouteMatchesMetricHandler(string path, Type expectedHandlerType, string? expectedMetricPath = null)
    {
        var resolver = _router.GetResolver(new ItemPath(path));
        resolver.HandlerType.Should().Be(expectedHandlerType);

        if (expectedMetricPath != null)
        {
            var services = new ServiceCollection();
            resolver.ServiceRegistrations.Invoke(services);
            var container = services.BuildServiceProvider();
            var metricName = container.GetRequiredService<MetricName>();
            metricName.NamespaceAndName.FullName.Should().Be(expectedMetricPath);
        }
    }
}