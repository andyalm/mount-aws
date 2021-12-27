using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Ec2;

public class InstanceItem : AwsItem
{
    public InstanceItem(string parentPath, PSObject instance) : base(parentPath, instance)
    {
        Name = Property<IEnumerable<PSObject>>("Tags")!
            .SingleOrDefault(t =>
                t.Property<string>("Key")?.Equals("Name") == true)?.Property<string>("Value");
    }

    public string? Name { get; }
    public override string ItemName => Property<string>("InstanceId")!;
    public override string ItemType => Ec2ItemTypes.Instance;
    public override bool IsContainer => false;
    public override IEnumerable<string> Aliases
    {
        get
        {
            var privateIpAddress = Property<string>("PrivateIpAddress");
            if (!string.IsNullOrEmpty(privateIpAddress))
            {
                yield return privateIpAddress;
            }
        }
    }

    public override void CustomizePSObject(PSObject psObject)
    {
        psObject.Properties.Add(new PSNoteProperty(nameof(Name), Name));
        base.CustomizePSObject(psObject);
    }
}