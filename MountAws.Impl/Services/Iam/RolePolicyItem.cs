using System.Management.Automation;
using System.Net;
using Amazon.IdentityManagement.Model;
using MountAnything;

namespace MountAws.Services.Iam;

public class RolePolicyItem : AwsItem
{
    public RolePolicyItem(ItemPath parentPath, GetRolePolicyResponse rolePolicy) : base(parentPath, new PSObject())
    {
        ItemName = rolePolicy.PolicyName;
        Document = WebUtility.UrlDecode(rolePolicy.PolicyDocument);
        ItemType = IamItemTypes.EmbeddedPolicy;
        WebUrl = WebUrlBuilder.Regionless()
            .CombineWith($"iam/home#/roles/{rolePolicy.RoleName}$jsonEditor?policyName={rolePolicy.PolicyName}");
    }

    public RolePolicyItem(ItemPath parentPath, RolePolicyAttachment policyVersion) : base(parentPath, new PSObject())
    {
        ItemName = policyVersion.PolicyName;
        VersionId = policyVersion.PolicyVersion.VersionId;
        PolicyArn = policyVersion.PolicyArn;
        Document = WebUtility.UrlDecode(policyVersion.PolicyVersion.Document);
        ItemType = IamItemTypes.PolicyAttachment;
        WebUrl = WebUrlBuilder.Regionless()
            .CombineWith($"iam/home#/policies/{policyVersion.PolicyArn}");
    }

    public override string ItemName { get; }

    [ItemProperty]
    public string? VersionId { get; }
    
    [ItemProperty]
    public string Document { get; }
    
    [ItemProperty]
    public string? PolicyArn { get; }
    public override string ItemType { get; }
    public override bool IsContainer => false;
    public override string? WebUrl { get; }
}