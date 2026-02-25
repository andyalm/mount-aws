using Amazon.ApplicationAutoScaling;

namespace MountAws.Services.Autoscaling;

public class KnownAutoscalingService(string serviceNamespace, string[] supportedDimensions)
{
    public static KnownAutoscalingService[] All { get; } =
    [
        DynamoDB
    ];
    
    public static KnownAutoscalingService DynamoDB { get; } = new("dynamodb", [
        "dynamodb:table:ReadCapacityUnits",
        "dynamodb:table:WriteCapacityUnits"
    ]);

    public ServiceNamespace ServiceNamespace => serviceNamespace;
    public string[] SupportedDimensions => supportedDimensions;
}