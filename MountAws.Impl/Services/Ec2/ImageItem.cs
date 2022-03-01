using Amazon.EC2.Model;
using MountAnything;

namespace MountAws.Services.Ec2;

public class ImageItem : AwsItem<Image>
{
    public ImageItem(ItemPath parentPath, Image underlyingObject) : base(parentPath, underlyingObject)
    {
        ItemName = underlyingObject.ImageId;
    }

    public override string ItemName { get; }
    public override bool IsContainer => false;

    public override IEnumerable<string> Aliases
    {
        get
        {
            if (!string.IsNullOrEmpty(UnderlyingObject.Name))
            {
                yield return UnderlyingObject.Name;
            }
        }
    }
}