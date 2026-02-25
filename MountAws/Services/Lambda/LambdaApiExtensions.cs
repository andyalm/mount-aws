using Amazon.Lambda;
using Amazon.Lambda.Model;
using static MountAws.PagingHelper;

namespace MountAws.Services.Lambda;

public static class LambdaApiExtensions
{
    public static IEnumerable<FunctionConfiguration> ListFunctions(this IAmazonLambda lambda)
    {
        return Paginate(nextToken =>
        {
            var response = lambda.ListFunctionsAsync(new ListFunctionsRequest
            {
                Marker = nextToken
            }).GetAwaiter().GetResult();

            return (response.Functions, response.NextMarker);
        });
    }

    public static FunctionConfiguration GetFunction(this IAmazonLambda lambda, string functionName)
    {
        var response = lambda.GetFunctionAsync(new GetFunctionRequest
        {
            FunctionName = functionName
        }).GetAwaiter().GetResult();

        return response.Configuration;
    }

    public static IEnumerable<AliasConfiguration> ListAliases(
        this IAmazonLambda lambda, string functionName)
    {
        return Paginate(nextToken =>
        {
            var response = lambda.ListAliasesAsync(new ListAliasesRequest
            {
                FunctionName = functionName,
                Marker = nextToken
            }).GetAwaiter().GetResult();

            return (response.Aliases, response.NextMarker);
        });
    }

    public static AliasConfiguration GetAlias(
        this IAmazonLambda lambda, string functionName, string aliasName)
    {
        var response = lambda.GetAliasAsync(new GetAliasRequest
        {
            FunctionName = functionName,
            Name = aliasName
        }).GetAwaiter().GetResult();

        return new AliasConfiguration
        {
            AliasArn = response.AliasArn,
            Description = response.Description,
            FunctionVersion = response.FunctionVersion,
            Name = response.Name,
            RevisionId = response.RevisionId,
            RoutingConfig = response.RoutingConfig
        };
    }

    public static IEnumerable<FunctionConfiguration> ListVersionsByFunction(
        this IAmazonLambda lambda, string functionName)
    {
        return Paginate(nextToken =>
        {
            var response = lambda.ListVersionsByFunctionAsync(
                new ListVersionsByFunctionRequest
            {
                FunctionName = functionName,
                Marker = nextToken
            }).GetAwaiter().GetResult();

            return (response.Versions, response.NextMarker);
        });
    }

    public static IEnumerable<EventSourceMappingConfiguration> ListEventSourceMappings(
        this IAmazonLambda lambda, string functionName)
    {
        return Paginate(nextToken =>
        {
            var response = lambda.ListEventSourceMappingsAsync(
                new ListEventSourceMappingsRequest
            {
                FunctionName = functionName,
                Marker = nextToken
            }).GetAwaiter().GetResult();

            return (response.EventSourceMappings, response.NextMarker);
        });
    }

    public static EventSourceMappingConfiguration GetEventSourceMapping(
        this IAmazonLambda lambda, string uuid)
    {
        var response = lambda.GetEventSourceMappingAsync(new GetEventSourceMappingRequest
        {
            UUID = uuid
        }).GetAwaiter().GetResult();

        return new EventSourceMappingConfiguration
        {
            UUID = response.UUID,
            EventSourceArn = response.EventSourceArn,
            FunctionArn = response.FunctionArn,
            State = response.State,
            StateTransitionReason = response.StateTransitionReason,
            BatchSize = response.BatchSize,
            LastModified = response.LastModified,
            LastProcessingResult = response.LastProcessingResult,
            MaximumBatchingWindowInSeconds = response.MaximumBatchingWindowInSeconds
        };
    }

    public static IEnumerable<LayersListItem> ListLayers(this IAmazonLambda lambda)
    {
        return Paginate(nextToken =>
        {
            var response = lambda.ListLayersAsync(new ListLayersRequest
            {
                Marker = nextToken
            }).GetAwaiter().GetResult();

            return (response.Layers, response.NextMarker);
        });
    }

    public static IEnumerable<LayerVersionsListItem> ListLayerVersions(
        this IAmazonLambda lambda, string layerName)
    {
        return Paginate(nextToken =>
        {
            var response = lambda.ListLayerVersionsAsync(new ListLayerVersionsRequest
            {
                LayerName = layerName,
                Marker = nextToken
            }).GetAwaiter().GetResult();

            return (response.LayerVersions, response.NextMarker);
        });
    }

    public static GetLayerVersionResponse GetLayerVersion(
        this IAmazonLambda lambda, string layerName, long versionNumber)
    {
        return lambda.GetLayerVersionAsync(new GetLayerVersionRequest
        {
            LayerName = layerName,
            VersionNumber = versionNumber
        }).GetAwaiter().GetResult();
    }
}
