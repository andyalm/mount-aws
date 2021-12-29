using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Route53;

public class ResourceRecordItem : AwsItem
{
    public ResourceRecordItem(string parentPath, PSObject record) : base(parentPath, record)
    {
        ItemName = Property<string>("Name")!;
        TargetDescription = BuildTargetDescription();
    }

    public string TargetDescription { get; }
    public override string ItemName { get; }
    public override bool IsContainer => false;

    public override void CustomizePSObject(PSObject psObject)
    {
        psObject.Properties.Add(new PSNoteProperty(nameof(TargetDescription), TargetDescription));
        base.CustomizePSObject(psObject);
    }

    private string BuildTargetDescription()
    {
        var type = Property<PSObject>("Type")!.Property<string>("Value");
        return type switch
        {
            "A" => AliasTargetDescription(),
            "CNAME" => CNameTargetDescription(),
            _ => ""
        };
    }

    private string CNameTargetDescription()
    {
        return Property<IEnumerable<PSObject>>("ResourceRecords")?.FirstOrDefault()
            ?.Property<string>("Value") ?? "";
    }

    private string AliasTargetDescription()
    {
        return Property<PSObject>("AliasTarget")?.Property<string>("DNSName") ?? "";
    }
}