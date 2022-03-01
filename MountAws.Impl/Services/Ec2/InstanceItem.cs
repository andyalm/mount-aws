using System.Management.Automation;
using Amazon.EC2.Model;
using MountAnything;

namespace MountAws.Services.Ec2;

public class InstanceItem : AwsItem<Instance>
{
    public InstanceItem(ItemPath parentPath, Instance instance, LinkGenerator linkGenerator) : base(parentPath, instance)
    {
        Name = UnderlyingObject.Tags
            .SingleOrDefault(t =>
                t.Key.Equals("Name"))?.Value;
        var asgName = UnderlyingObject.Tags
            .SingleOrDefault(t =>
                t.Key.Equals("aws:autoscaling:groupName"))?.Value;
        if (!string.IsNullOrEmpty(asgName))
        {
            LinkPaths = new Dictionary<string, ItemPath>
            {
                ["AutoScalingGroup"] = linkGenerator.AutoScalingGroup(asgName),
                ["Image"] = linkGenerator.Ec2Image(instance.ImageId)
            };
        }
    }

    [ItemProperty]
    public string? Name { get; }

    [ItemProperty]
    public string State => UnderlyingObject.State.Name;

    [ItemProperty] 
    public int StateCode => UnderlyingObject.State.Code;
    public override string ItemName => UnderlyingObject.InstanceId;
    public override string ItemType => Ec2ItemTypes.Instance;
    public override bool IsContainer => false;

    public override IEnumerable<string> Aliases
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

    public override string? WebUrl =>
        UrlBuilder.CombineWith($"ec2/v2/home?#InstanceDetails:instanceId={UnderlyingObject.InstanceId}");
}