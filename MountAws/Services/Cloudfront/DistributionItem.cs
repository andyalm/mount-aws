using System.Management.Automation;
using Amazon.CloudFront.Model;
using MountAnything;
using MountAws.Services.Wafv2;

namespace MountAws.Services.Cloudfront;

public class DistributionItem : AwsItem<DistributionSummary>
{
    public DistributionItem(ItemPath parentPath, DistributionSummary distribution, LinkGenerator linkGenerator) : base(parentPath, distribution)
    {
        ItemName = distribution.Id;
        LinkPaths = new Dictionary<string, ItemPath>();
        if (distribution.WebACLId?.StartsWith("arn:") == true)
        {
            LinkPaths["WebAcl"] = linkGenerator.Wafv2CloudfrontWebAcl(distribution.WebACLId);
        }
    }

    public override string ItemName { get; }
    public override bool IsContainer => false;
    public string DomainName => UnderlyingObject.DomainName;

    public override string? WebUrl =>
        WebUrlBuilder.Regionless().CombineWith($"cloudfront/v3/home#/distributions/{ItemName}");

    public override IEnumerable<string> Aliases
    {
        get
        {
            yield return DomainName;
        }
    }
}