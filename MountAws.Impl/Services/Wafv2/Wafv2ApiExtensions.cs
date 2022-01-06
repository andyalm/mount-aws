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

    public static WebACL GetWebAcl(this IAmazonWAFV2 wafv2, Scope scope, string name)
    {
        return wafv2.GetWebACLAsync(new GetWebACLRequest
        {
            Scope = scope,
            Name = name
        }).GetAwaiter().GetResult().WebACL;
    }
}