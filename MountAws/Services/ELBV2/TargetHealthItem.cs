using System.Management.Automation;
using Amazon.ElasticLoadBalancingV2.Model;

namespace MountAws.Services.ELBV2;

public class TargetHealthItem : AwsItem
{
    private readonly string _parentPath;
    public TargetHealthDescription TargetHealth { get; }

    public TargetHealthItem(string parentPath, TargetHealthDescription targetHealth)
    {
        _parentPath = parentPath;
        TargetHealth = targetHealth;
    }

    public override string FullPath => AwsPath.Combine(_parentPath, ItemName);

    public override string ItemName => TargetHealth.Target.Port > 0
        ? $"{TargetHealth.Target.Id}:{TargetHealth.Target.Port}"
        : TargetHealth.Target.Id.Replace("/", "|");

    public override object UnderlyingObject => TargetHealth;
    public override string ItemType => "Target";
    public override bool IsContainer => false;

    public string Id => TargetHealth.Target.Id;
    public string? Port => TargetHealth.Target.Port > 0 ? TargetHealth.Target.Port.ToString() : null;
    public string? HealthStatus => TargetHealth.TargetHealth?.State?.Value;
    public string? HealthReason => TargetHealth.TargetHealth?.Reason?.Value;
    public string? HealthDescription => TargetHealth.TargetHealth?.Description;

    public override void CustomizePSObject(PSObject psObject)
    {
        base.CustomizePSObject(psObject);
        psObject.Properties.Add(new PSNoteProperty(nameof(Id), Id));
        psObject.Properties.Add(new PSNoteProperty(nameof(Port), Port));
        psObject.Properties.Add(new PSNoteProperty(nameof(HealthStatus), HealthStatus));
        psObject.Properties.Add(new PSNoteProperty(nameof(HealthReason), HealthReason));
        psObject.Properties.Add(new PSNoteProperty(nameof(HealthDescription), HealthDescription));
    }
}