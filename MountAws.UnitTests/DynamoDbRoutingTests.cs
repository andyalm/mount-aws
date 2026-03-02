using System;
using AwesomeAssertions;
using MountAnything;
using MountAnything.Routing;
using MountAws.Services.DynamoDb;
using Xunit;

namespace MountAws.UnitTests;

public class DynamoDbRoutingTests
{
    private readonly Router _router;

    public DynamoDbRoutingTests()
    {
        var provider = new MountAwsProvider();
        _router = provider.CreateRouter();
    }

    [Theory]
    [InlineData("myprofile/us-east-1/dynamodb", typeof(DynamoDbRootHandler))]
    [InlineData("myprofile/us-east-1/dynamodb/tables", typeof(TablesHandler))]
    [InlineData("myprofile/us-east-1/dynamodb/tables/my-table", typeof(TableHandler))]
    [InlineData("myprofile/us-east-1/dynamodb/tables/my-table/autoscaling", typeof(TableAutoscalingHandler))]
    [InlineData("myprofile/us-east-1/dynamodb/tables/my-table/items", typeof(TableItemsHandler))]
    public void DynamoDbRoutesResolveToCorrectHandlers(string path, Type expectedHandlerType)
    {
        var resolver = _router.GetResolver(new ItemPath(path));
        resolver.HandlerType.Should().Be(expectedHandlerType);
    }
}
