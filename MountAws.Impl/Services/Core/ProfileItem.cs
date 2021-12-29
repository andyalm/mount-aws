using Amazon.Runtime.CredentialManagement;

namespace MountAws.Services.Core;

public class ProfileItem : AwsItem<CredentialProfile>
{
    public ProfileItem(CredentialProfile profile) : base(string.Empty, profile)
    {
        
    }
    public override string ItemName => UnderlyingObject.Name;
    public override string ItemType => CoreItemTypes.Profile;
    public override bool IsContainer => true;
}