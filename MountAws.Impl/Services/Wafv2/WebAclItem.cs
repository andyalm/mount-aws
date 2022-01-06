using System.Management.Automation;
using Amazon.WAFV2;
using Amazon.WAFV2.Model;
using MountAnything;

namespace MountAws.Services.Wafv2;

public class WebAclItem : AwsItem
{
    public WebAclItem(ItemPath parentPath, WebACLSummary summary) : base(parentPath, new PSObject(new
    {
        summary.Id,
        summary.Name,
        summary.Description,
        summary.ARN
    }))
    {
        ItemName = summary.Name;
    }

    public WebAclItem(ItemPath parentPath, WebACL acl) : base(parentPath, new PSObject(new
    {
        acl.Id,
        acl.Name,
        acl.Description,
        acl.ARN
    }))
    {
        ItemName = acl.Name;
    }

    public override string ItemName { get; }
    
    public override bool IsContainer => true;
}