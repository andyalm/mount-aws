using Amazon.WAFV2;
using Amazon.WAFV2.Model;

using static MountAws.PagingHelper;

namespace MountAws.Services.Wafv2;

public static class Wafv2ApiExtensions
{
    public static IEnumerable<WebACLSummary> ListWebAcls(this IAmazonWAFV2 wafv2, Scope scope)
    {
        return Paginate(nextToken =>
        {
            var response = wafv2.ListWebACLsAsync(new ListWebACLsRequest
            {
                Scope = scope,
                NextMarker = nextToken
            }).GetAwaiter().GetResult();

            return (response.WebACLs, response.NextMarker);
        });
    }

    public static WebACL GetWebAcl(this IAmazonWAFV2 wafv2, Scope scope, (string Id, string Name) identifier)
    {
        return wafv2.GetWebACLAsync(new GetWebACLRequest
        {
            Scope = scope,
            Name = identifier.Name,
            Id = identifier.Id
        }).GetAwaiter().GetResult().WebACL;
    }

    public static RegexPatternSet GetRegexPatternSet(this IAmazonWAFV2 wafv2, string arn)
    {
        var parts = arn.Split(":").Last().Split("/");
        var scope = parts[0];
        var name = parts[2];
        var id = parts[3];
        return wafv2.GetRegexPatternSetAsync(new GetRegexPatternSetRequest
        {
            Id = id,
            Name = name,
            Scope = scope == "global" ? Scope.CLOUDFRONT : Scope.REGIONAL
        }).GetAwaiter().GetResult().RegexPatternSet;
    }

    public static IPSet GetIPSet(this IAmazonWAFV2 wafv2, string arn)
    {
        var parts = arn.Split(":").Last().Split("/");
        var scope = parts[0];
        var name = parts[2];
        var id = parts[3];
        return wafv2.GetIPSetAsync(new GetIPSetRequest
        {
            Id = id,
            Name = name,
            Scope = scope == "global" ? Scope.CLOUDFRONT : Scope.REGIONAL
        }).GetAwaiter().GetResult().IPSet;
    }
}