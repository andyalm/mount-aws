using System.Management.Automation;
using Amazon.IdentityManagement.Model;
using MountAnything;

namespace MountAws.Services.Iam;

public class UserItem : AwsItem
{
    public UserItem(ItemPath parentPath, User underlyingObject) : base(parentPath, new PSObject(underlyingObject))
    {
        ItemName = underlyingObject.UserName;
        ItemType = IamItemTypes.User;

        WebUrl = WebUrlBuilder.Regionless().CombineWith($"iamv2/home?#/users/details/{underlyingObject.UserName}").ToString();
    }
    
    public UserItem(ItemPath parentPath, string path) : base(parentPath, new PSObject(new
    {
        Path = path
    }))
    {
        ItemName = path.Split("/").Last();
        ItemType = IamItemTypes.Directory;
    }

    public override string ItemName { get; }
    public override bool IsContainer => true;
    public override string ItemType { get; }
    public override string? WebUrl { get; }
}