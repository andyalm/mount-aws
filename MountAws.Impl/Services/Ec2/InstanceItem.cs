using System.Management.Automation;
using Amazon.EC2.Model;
using MountAnything;

namespace MountAws.Services.Ec2;

public class InstanceItem : AwsItem<Instance>
{
    public InstanceItem(ItemPath parentPath, Instance instance) : base(parentPath, instance)
    {
        Name = UnderlyingObject.Tags
            .SingleOrDefault(t =>
                t.Key.Equals("Name"))?.Value;
    }

    public string? Name { get; }
    public override string ItemName => UnderlyingObject.InstanceId;
    public override string ItemType => Ec2ItemTypes.Instance;
    public override bool IsContainer => false;

    protected override IEnumerable<string> Aliases
    {
        get
        {
            var privateIpAddress = UnderlyingObject.PrivateIpAddress;
            if (!string.IsNullOrEmpty(privateIpAddress))
            {
                yield return privateIpAddress;
            }
        }
    }

    protected override void CustomizePSObject(PSObject psObject)
    {
        psObject.Properties.Add(new PSNoteProperty(nameof(Name), Name));
        base.CustomizePSObject(psObject);
    }
}