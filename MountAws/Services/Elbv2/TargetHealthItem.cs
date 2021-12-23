using System.Management.Automation;
using MountAnything;
using MountAws.Api;

namespace MountAws.Services.Elbv2;

public class TargetHealthItem : Item
{
    private PSObject Target { get; }
    private PSObject? TargetHealth { get; }
    public TargetHealthItem(string parentPath, PSObject targetHealth) : base(parentPath, targetHealth)
    {
        Target = Property<PSObject>("Target")!;
        if (Target.Property<int>("Port") > 0)
        {
            ItemName = $"{Target.Property<string>("Id")}:{Target.Property<int>("Port")}";
            Port = Target.Property<string>("Port");
        }
        else
        {
            ItemName = Target.Property<string>("Id")!.Replace("/", "|");
        }

        TargetHealth = Property<PSObject>("TargetHealth");
    }

    public override string ItemName { get; }
    public override string ItemType => "Target";
    public override bool IsContainer => false;

    public string Id => Target.Property<string>("Id")!;
    public string? Port { get; }
    public string? HealthStatus => TargetHealth?.Property<object>("State")?.ToString();
    public string? HealthReason => TargetHealth?.Property<object>("Reason")?.ToString();
    public string? HealthDescription => TargetHealth?.Property<string>("Description");

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