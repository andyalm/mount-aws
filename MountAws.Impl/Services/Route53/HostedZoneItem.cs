using System.Management.Automation;

namespace MountAws.Services.Route53;

public class HostedZoneItem : AwsItem
{
    public HostedZoneItem(string parentPath, PSObject hostedZone) : base(parentPath, hostedZone)
    {
        ItemName = Property<string>("Id")!.Split("/").Last();
        Name = Property<string>("Name")!;
    }

    public override string ItemName { get; }
    public string Name { get; }
    public override bool IsContainer => true;

    public override IEnumerable<string> Aliases
    {
        get
        {
            yield return Name;
        }
    }

    public override void CustomizePSObject(PSObject psObject)
    {
        psObject.Properties.Add(new PSNoteProperty(nameof(Name), Name));
        psObject.Properties.Add(new PSAliasProperty("ShortId", "ItemName"));
        base.CustomizePSObject(psObject);
    }
}