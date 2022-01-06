using System.Management.Automation;
using Amazon.WAFV2;
using Amazon.WAFV2.Model;
using MountAnything;

namespace MountAws.Services.Wafv2;

public class WebAclItem : AwsItem<WebACLSummary>
{
    public WebAclItem(ItemPath parentPath, WebACLSummary summary) : base(parentPath, summary)
    {
        ItemName = summary.Name;
    }

    public override string ItemName { get; }
    public string Id => UnderlyingObject.Id;
    public override bool IsContainer => true;
}