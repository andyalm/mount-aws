using System.Management.Automation;
using MountAnything;

namespace MountAws.Services.Ec2;

public class InstanceItem : Item
{
    public InstanceItem(string parentPath, PSObject instance) : base(parentPath, instance)
    {
        
    }

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

            var nameTag = Property<IEnumerable<PSObject>>("Tags")!
                .SingleOrDefault(t =>
                    t.Property<string>("Key")?.Equals("Name", StringComparison.OrdinalIgnoreCase) == true);
            if (nameTag != null)
            {
                yield return nameTag.Property<string>("Value")!;
            }
        }
    }
}