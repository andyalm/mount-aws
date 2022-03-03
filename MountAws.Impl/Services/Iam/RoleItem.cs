using System.Management.Automation;
using System.Net;
using Amazon.IdentityManagement.Model;
using MountAnything;

namespace MountAws.Services.Iam;

public class RoleItem : AwsItem
{
    public RoleItem(ItemPath parentPath, Role underlyingObject) : base(parentPath, new PSObject(underlyingObject))
    {
        ItemName = underlyingObject.RoleName;
        ItemType = IamItemTypes.Role;
        LastUsedDate = underlyingObject.RoleLastUsed?.LastUsedDate;
        if (underlyingObject.AssumeRolePolicyDocument.StartsWith("%"))
        {
            UnderlyingObject.Properties.Remove(nameof(Role.AssumeRolePolicyDocument));
            UnderlyingObject.Properties.Add(new PSNoteProperty(nameof(Role.AssumeRolePolicyDocument), WebUtility.UrlDecode(underlyingObject.AssumeRolePolicyDocument)));
        }
    }
    
    public RoleItem(ItemPath parentPath, string path) : base(parentPath, new PSObject(new
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
    
    [ItemProperty]
    public DateTime? LastUsedDate { get; }
}