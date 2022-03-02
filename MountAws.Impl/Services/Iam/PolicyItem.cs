using System.Management.Automation;
using Amazon.IdentityManagement.Model;
using MountAnything;

namespace MountAws.Services.Iam;

public class PolicyItem : AwsItem
{
    public PolicyItem(ItemPath parentPath, ManagedPolicy underlyingObject) : base(parentPath, new PSObject(underlyingObject))
    {
        ItemName = underlyingObject.PolicyName;
        ItemType = IamItemTypes.Policy;
    }
    
    public PolicyItem(ItemPath parentPath, string path) : base(parentPath, new PSObject(new
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
}