using Amazon.Runtime.CredentialManagement;
using MountAnything;

namespace MountAws.Services.Core;

public class ProfileItem : AwsItem<CredentialProfile>
{
    public ProfileItem(CredentialProfile profile) : base(ItemPath.Root, profile)
    {
        
    }
    public override string ItemName => UnderlyingObject.Name;
    public override string ItemType => CoreItemTypes.Profile;
    public override bool IsContainer => true;
}