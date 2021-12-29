using System.Management.Automation;
using Amazon.Route53.Model;

namespace MountAws.Services.Route53;

public class HostedZoneItem : AwsItem<HostedZone>
{
    public HostedZoneItem(string parentPath, HostedZone hostedZone) : base(parentPath, hostedZone)
    {
        ItemName = hostedZone.Id.Split("/").Last();
        Name = hostedZone.Name;
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