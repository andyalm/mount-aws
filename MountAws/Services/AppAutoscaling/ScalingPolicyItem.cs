using System.Globalization;
using Amazon.ApplicationAutoScaling.Model;
using MountAnything;

namespace MountAws.Services.AppAutoscaling;

public class ScalingPolicyItem(ItemPath parentPath, ScalingPolicy underlyingObject)
    : AwsItem<ScalingPolicy>(parentPath, underlyingObject)
{
    public override string ItemName => UnderlyingObject.PolicyName;
    public override bool IsContainer => false;

    [ItemProperty]
    public string ScalableDimension => UnderlyingObject.ScalableDimension.Value;

    [ItemProperty]
    public string Summary => UnderlyingObject switch
    {
        { TargetTrackingScalingPolicyConfiguration: not null } => GetTargetTrackingSummary(UnderlyingObject
            .TargetTrackingScalingPolicyConfiguration),
        { PredictiveScalingPolicyConfiguration: not null } => GetPredictiveScalingSummary(UnderlyingObject
            .PredictiveScalingPolicyConfiguration),
        { StepScalingPolicyConfiguration: not null } => GetStepScalingSummary(UnderlyingObject
            .StepScalingPolicyConfiguration),
        _ => UnderlyingObject.PolicyType.Value
    };

    private string GetTargetTrackingSummary(TargetTrackingScalingPolicyConfiguration targetTracking)
    {
        return targetTracking switch
        {
            { PredefinedMetricSpecification: not null } =>
                $"{targetTracking.PredefinedMetricSpecification.PredefinedMetricType} {targetTracking.TargetValue}",
            { CustomizedMetricSpecification: not null } =>
                $"{targetTracking.CustomizedMetricSpecification.MetricName} {targetTracking.TargetValue}",
            _ => targetTracking.TargetValue.ToString(CultureInfo.CurrentCulture)
        };
    }
    
    private string GetPredictiveScalingSummary(PredictiveScalingPolicyConfiguration predectiveScaling)
    {
        return predectiveScaling.Mode.Value;
    }
    
    private string GetStepScalingSummary(StepScalingPolicyConfiguration stepScaling)
    {
        return $"{stepScaling.MetricAggregationType} {stepScaling.AdjustmentType}";
    }
}
