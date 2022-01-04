using System.Management.Automation;
using Amazon.Route53.Model;
using MountAnything;

namespace MountAws.Services.Route53;

public class ResourceRecordItem : AwsItem<ResourceRecordSet>
{
    public ResourceRecordItem(ItemPath parentPath, ResourceRecordSet record) : base(parentPath, record)
    {
        ItemName = record.Name;
        TargetDescription = BuildTargetDescription();
    }

    public string TargetDescription { get; }
    public override string ItemName { get; }
    public override bool IsContainer => false;

    protected override void CustomizePSObject(PSObject psObject)
    {
        psObject.Properties.Add(new PSNoteProperty(nameof(TargetDescription), TargetDescription));
        base.CustomizePSObject(psObject);
    }

    private string BuildTargetDescription()
    {
        return UnderlyingObject.AliasTarget?.DNSName ?? UnderlyingObject.ResourceRecords?.FirstOrDefault()?.Value ?? "";
    }
}