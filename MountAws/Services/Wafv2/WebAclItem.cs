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

    public override string? WebUrl => UnderlyingObject.IsGlobal() ?
        WebUrlBuilder.Regionless().CombineWith($"wafv2/homev2/web-acl/{UnderlyingObject.Name}/{UnderlyingObject.Id}/overview?region=global") :
        WebUrlBuilder.Regionless().CombineWith($"wafv2/homev2/web-acl/{UnderlyingObject.Name}/{UnderlyingObject.Id}/overview?region={UnderlyingObject.RegionName()}");
}