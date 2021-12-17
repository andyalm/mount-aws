using Amazon.EC2.Model;
using MountAnything;

namespace MountAws.Services.EC2;

public class EC2InstanceItem : Item
{
    private readonly Instance _ec2Instance;
    
    public EC2InstanceItem(string parentPath, Instance instance) : base(parentPath)
    {
        _ec2Instance = instance;
    }

    public override string ItemName => _ec2Instance.InstanceId;
    public override object UnderlyingObject => _ec2Instance;
    public override string ItemType => "Instance";
    public override bool IsContainer => false;

    public override IEnumerable<string> Aliases
    {
        get
        {
            if (!string.IsNullOrEmpty(_ec2Instance.PrivateIpAddress))
            {
                yield return _ec2Instance.PrivateIpAddress;
            }

            var name = _ec2Instance.Name();
            if (!string.IsNullOrEmpty(name))
            {
                yield return name;
            }
        }
    }
}