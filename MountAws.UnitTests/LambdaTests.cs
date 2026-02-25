using AwesomeAssertions;
using MountAnything;
using MountAnything.Routing;
using MountAws.Services.Lambda;
using Xunit;

namespace MountAws.UnitTests;

public class LambdaTests
{
    private readonly Router _router;

    public LambdaTests()
    {
        var provider = new MountAwsProvider();
        _router = provider.CreateRouter();
    }

    [Theory]
    [InlineData("myprofile/us-east-1/lambda", typeof(LambdaRootHandler))]
    [InlineData("myprofile/us-east-1/lambda/functions", typeof(FunctionsHandler))]
    [InlineData("myprofile/us-east-1/lambda/functions/my-function", typeof(FunctionHandler))]
    [InlineData("myprofile/us-east-1/lambda/functions/my-function/aliases", typeof(AliasesHandler))]
    [InlineData("myprofile/us-east-1/lambda/functions/my-function/aliases/prod", typeof(AliasHandler))]
    [InlineData("myprofile/us-east-1/lambda/functions/my-function/versions", typeof(VersionsHandler))]
    [InlineData("myprofile/us-east-1/lambda/functions/my-function/versions/3", typeof(VersionHandler))]
    [InlineData("myprofile/us-east-1/lambda/functions/my-function/event-source-mappings", typeof(EventSourceMappingsHandler))]
    [InlineData("myprofile/us-east-1/lambda/functions/my-function/event-source-mappings/abc-123-def", typeof(EventSourceMappingHandler))]
    [InlineData("myprofile/us-east-1/lambda/layers", typeof(LayersHandler))]
    [InlineData("myprofile/us-east-1/lambda/layers/my-layer", typeof(LayerHandler))]
    [InlineData("myprofile/us-east-1/lambda/layers/my-layer/5", typeof(LayerVersionHandler))]
    public void LambdaRoutesResolveToCorrectHandlers(string path, Type expectedHandlerType)
    {
        var resolver = _router.GetResolver(new ItemPath(path));
        resolver.HandlerType.Should().Be(expectedHandlerType);
    }
}
