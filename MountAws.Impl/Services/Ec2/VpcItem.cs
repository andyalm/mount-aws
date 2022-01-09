using Amazon.EC2.Model;
using MountAnything;

namespace MountAws.Services.Ec2;

public class VpcItem : AwsItem<Vpc>
{
    public VpcItem(ItemPath parentPath, Vpc underlyingObject) : base(parentPath, underlyingObject)
    {
        ItemName = UnderlyingObject.VpcId;
    }

    public override string ItemName { get; }
    public override bool IsContainer => true;

    public override string? WebUrl => UrlBuilder.CombineWith($"vpc/home#VpcDetails:VpcId={UnderlyingObject.VpcId}");
}