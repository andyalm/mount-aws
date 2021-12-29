using System.Management.Automation;
using Amazon.ElasticLoadBalancingV2.Model;

namespace MountAws.Services.Elbv2;

public class TargetHealthItem : AwsItem<TargetHealthDescription>
{
    private TargetDescription Target { get; }
    private TargetHealth TargetHealth { get; }
    public TargetHealthItem(string parentPath, TargetHealthDescription targetHealth) : base(parentPath, targetHealth)
    {
        Target = targetHealth.Target;
        if (Target.Port > 0)
        {
            ItemName = $"{Target.Id}:{Target.Port}";
            Port = Target.Port.ToString();
        }
        else
        {
            ItemName = Target.Id.Replace("/", "|");
        }

        TargetHealth = targetHealth.TargetHealth;
    }

    public override string ItemName { get; }
    public override string ItemType => "Target";
    public override bool IsContainer => false;

    public string Id => Target.Id;
    public string? Port { get; }
    public string? HealthStatus => TargetHealth?.State?.ToString();
    public string? HealthReason => TargetHealth?.Reason?.ToString();
    public string? HealthDescription => TargetHealth?.Description;

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